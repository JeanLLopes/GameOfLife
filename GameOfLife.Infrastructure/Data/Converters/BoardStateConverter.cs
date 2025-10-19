using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace GameOfLife.Infrastructure.Data.Converters;

/// <summary>
/// Converte o estado bool[][] para uma string JSON para armazenamento no banco de dados
/// e vice-versa.
/// </summary>
public class BoardStateConverter : ValueConverter<bool[][], string>
{
    public BoardStateConverter() : base(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
        v => JsonSerializer.Deserialize<bool[][]>(v, (JsonSerializerOptions)null!)!)
    {
    }
}