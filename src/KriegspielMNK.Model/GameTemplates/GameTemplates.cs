
using System.Reflection;
using KriegspielMNK.Model.Template;

namespace KriegspielMNK.Model;
public static partial class GameTemplates {
    public static IEnumerable<IGameTemplate> GetBuiltInGameTemplates()
        => typeof(GameTemplates)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(propInfo => propInfo.PropertyType.IsAssignableTo(typeof(IGameTemplate)))
            .Select(propInfo => (IGameTemplate)propInfo.GetValue(null)!);
}