using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CashFlow.Domain.Entities;

public class EntityControl
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [NotMapped]
    public string __TableName => GetType().Name.ToUpper();


    public bool NeedsUpdate(EntityControl old)
    {
        return !EqualsForUpdate(old);
    }

    public bool EqualsForUpdate(EntityControl obj)
    {
        EntityControl other = obj as EntityControl;
        if (other == null)
            return false;

        if (GetType() != other.GetType())
            return false;

        List<PropertyInfo> PropertyInfoList = obj.GetType().GetProperties()
            .Where(x => x.GetMethod != null && !x.GetMethod.IsPrivate
                     && x.SetMethod != null && !x.SetMethod.IsPrivate)
            .ToList();

        bool equals = true;
        PropertyInfoList.FirstOrDefault(PropertyInfo =>
        {
            //if (!DCUtils.IsPrimitiveOrEnum(PropertyInfo.PropertyType))
            //    return false;

            equals = PropertyEquals(PropertyInfo, other);

            return !equals;
        });

        return equals;
    }

    private bool PropertyEquals(PropertyInfo PropertyInfo, EntityControl other)
    {
        bool equals = true;

        object thisValue = PropertyInfo.GetValue(this);
        object otherValue = PropertyInfo.GetValue(other);

        if (thisValue == null && otherValue != null)
            equals = false;
        else if (thisValue != null && otherValue == null)
            equals = false;
        else if (thisValue != null && otherValue != null)
            equals = thisValue.Equals(otherValue);

        return equals;
    }

    public IEnumerable<PropertyInfo> GetChangedProperties(EntityControl entityOld, bool ForeignKeyAttributesNotIncluded = false)
    {
        List<PropertyInfo> listPropertyInfo = GetType().GetProperties()
            .Where(x => x.GetMethod != null && !x.GetMethod.IsPrivate
                     && x.SetMethod != null && !x.SetMethod.IsPrivate
                     && (x.GetCustomAttribute<NotMappedAttribute>() == null
                      || (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(List<>))))
            .ToList();

        if (ForeignKeyAttributesNotIncluded)
            listPropertyInfo = listPropertyInfo.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() == null).ToList();

        if (entityOld == null)
            return listPropertyInfo;

        if (GetType() != entityOld.GetType())
            return listPropertyInfo;

        List<PropertyInfo> listChangedProperties = new List<PropertyInfo>();

        listPropertyInfo.ForEach(pi =>
        {
            CheckAndAddChangedProperty(entityOld, pi, listChangedProperties);
        });

        return listChangedProperties;
    }

    public async System.Threading.Tasks.Task<IEnumerable<PropertyInfo>> GetChangedPropertiesAsync(EntityControl entityOld, bool ForeignKeyAttributesNotIncluded = false)
    {
        List<PropertyInfo> listChangedProperties = new List<PropertyInfo>();
        //await System.Threading.Tasks.Task.Run(() =>
        // {
        List<PropertyInfo> listPropertyInfo = GetType().GetProperties()
            .Where(x => x.GetMethod != null && !x.GetMethod.IsPrivate
                     && x.SetMethod != null && !x.SetMethod.IsPrivate
                     && (x.GetCustomAttribute<NotMappedAttribute>() == null
                      || (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(List<>))))
            .ToList();

        if (ForeignKeyAttributesNotIncluded)
            listPropertyInfo = listPropertyInfo.Where(x => x.GetCustomAttribute<ForeignKeyAttribute>() == null && x.GetCustomAttribute<NotMappedAttribute>() == null).ToList();

        if (entityOld == null)
        {
            //listChangedProperties = listPropertyInfo;
            return listPropertyInfo;
        }

        if (GetType() != entityOld.GetType())
        {
            // listChangedProperties = listPropertyInfo;
            return listPropertyInfo;
        }

        listChangedProperties = new List<PropertyInfo>();

        foreach (var pi in listPropertyInfo)
        {
            await CheckAndAddChangedPropertyAsync(entityOld, pi, listChangedProperties);
        }
        // });

        return listChangedProperties;
    }

    private void CheckAndAddChangedProperty(EntityControl entityOld, PropertyInfo pi, List<PropertyInfo> listChangedProperties)
    {
        object thisValue = pi.GetValue(this);
        object otherValue = pi.GetValue(entityOld);

        if (thisValue == null && otherValue != null)
            listChangedProperties.Add(pi);
        else if (thisValue != null && otherValue == null)
            listChangedProperties.Add(pi);
        else if (!(thisValue == null && otherValue == null))
        {
            if (typeof(IEnumerable<EntityControl>).IsAssignableFrom(pi.PropertyType))
                CheckAndAddChangedPropertyIEnumerable(pi, listChangedProperties, thisValue, otherValue);
            //else if (DCUtils.IsPrimitiveOrEnum(pi.PropertyType) && !thisValue.Equals(otherValue) && pi.Name != "UpdatedAt" && pi.Name != "CreatedAt")
            //    listChangedProperties.Add(pi);
        }
    }

    private async Task CheckAndAddChangedPropertyAsync(EntityControl entityOld, PropertyInfo pi, List<PropertyInfo> listChangedProperties)
    {
        object thisValue = pi.GetValue(this);
        object otherValue = pi.GetValue(entityOld);

        if (thisValue == null && otherValue != null)
            listChangedProperties.Add(pi);
        else if (thisValue != null && otherValue == null)
            listChangedProperties.Add(pi);
        else if (!(thisValue == null && otherValue == null))
        {
            if (typeof(IEnumerable<EntityControl>).IsAssignableFrom(pi.PropertyType))
                await CheckAndAddChangedPropertyIEnumerableAsync(pi, listChangedProperties, thisValue, otherValue);
            //else
            //if (DCUtils.IsPrimitiveOrEnum(pi.PropertyType))
            //{
            //    if (thisValue is DateTime && otherValue is DateTime)
            //    {
            //        if (((DateTime)thisValue).Ticks != (((DateTime)otherValue).Ticks))
            //        {
            //            long thisValueTick = (((DateTime)thisValue).Ticks / 10000000);
            //            long otherValueTick = (((DateTime)otherValue).Ticks / 10000000);

            //            if (thisValueTick == otherValueTick)
            //            {
            //                thisValue = ((DateTime)thisValue).AddTicks(-((DateTime)thisValue).Ticks).AddTicks(thisValueTick);
            //                otherValue = ((DateTime)otherValue).AddTicks(-((DateTime)otherValue).Ticks).AddTicks(otherValueTick);
            //            }
            //        }
            //    }

            //    if (!thisValue.Equals(otherValue))
            //        listChangedProperties.Add(pi);
            //}
        }
    }

    private void CheckAndAddChangedPropertyIEnumerable(PropertyInfo pi, List<PropertyInfo> listChangedProperties, object thisValue, object otherValue)
    {
        var listThis = thisValue as IEnumerable<EntityControl>;
        var listOther = otherValue as IEnumerable<EntityControl>;

        bool isEmptyThis = listThis.Any();
        bool isEmptyOther = listOther.Any();

        if (isEmptyThis && !isEmptyOther)
            listChangedProperties.Add(pi);
        else if (!isEmptyThis && isEmptyOther)
            listChangedProperties.Add(pi);
        else if (!isEmptyThis && !isEmptyOther)
        {
            var firstItem = listThis.First();
            string idPropertyName = $"ID".ToLower();
            PropertyInfo idPropertyInfo = firstItem.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals(idPropertyName));
            if (idPropertyInfo == null)
                throw new Exception($"Tabela {firstItem.GetType().Name} com o campo de ID fora do padrão");

            if (listThis.Any(thisOne => !listOther.Any(otherOne => thisOne.PropertyEquals(idPropertyInfo, otherOne))))
                listChangedProperties.Add(pi);
            else if (listOther.Any(otherOne => !listThis.Any(thisOne => otherOne.PropertyEquals(idPropertyInfo, thisOne))))
                listChangedProperties.Add(pi);
            else
            {
                foreach (EntityControl itemThis in listThis)
                {
                    var itemOther = listOther.FirstOrDefault(x => itemThis.PropertyEquals(idPropertyInfo, x));
                    if (itemThis.GetChangedProperties(itemOther).Count() > 0)
                        listChangedProperties.Add(pi);
                }
            }
        }
    }

    private async Task CheckAndAddChangedPropertyIEnumerableAsync(PropertyInfo pi, List<PropertyInfo> listChangedProperties, object thisValue, object otherValue)
    {
        var listThis = thisValue as IEnumerable<EntityControl>;
        var listOther = otherValue as IEnumerable<EntityControl>;

        bool isEmptyThis = listThis.Any();
        bool isEmptyOther = listOther.Any();

        if (isEmptyThis && !isEmptyOther)
            listChangedProperties.Add(pi);
        else if (!isEmptyThis && isEmptyOther)
            listChangedProperties.Add(pi);
        else if (!isEmptyThis && !isEmptyOther)
        {
            var firstItem = listThis.First();
            string idPropertyName = $"ID".ToLower();
            PropertyInfo idPropertyInfo = firstItem.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals(idPropertyName));
            if (idPropertyInfo == null)
                throw new Exception($"Tabela {firstItem.GetType().Name} com o campo de ID fora do padrão");

            if (listThis.Any(thisOne => !listOther.Any(otherOne => thisOne.PropertyEquals(idPropertyInfo, otherOne))))
                listChangedProperties.Add(pi);
            else if (listOther.Any(otherOne => !listThis.Any(thisOne => otherOne.PropertyEquals(idPropertyInfo, thisOne))))
                listChangedProperties.Add(pi);
            else
            {
                foreach (EntityControl itemThis in listThis)
                {
                    var itemOther = listOther.FirstOrDefault(x => itemThis.PropertyEquals(idPropertyInfo, x));
                    if ((await itemThis.GetChangedPropertiesAsync(itemOther)).Count() > 0)
                        listChangedProperties.Add(pi);
                }
            }
        }
    }
}
