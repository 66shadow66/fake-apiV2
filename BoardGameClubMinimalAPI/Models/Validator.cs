using System.Text.RegularExpressions;
using BoardGameClubMinimalAPI.Data;

namespace BoardGameClubMinimalAPI.Models;

public static class Validator
{
    private const string EmailRegex = @"^[\w-]+@[\w-]+\.\w+$";

    public static bool ValidateString(string value, int minLength, string fieldName, out string error)
    {
        if (string.IsNullOrEmpty(value))
        {
            error = $"{fieldName} не може бути порожнім.";
            return false;
        }
        if (value.Length < minLength)
        {
            error = $"{fieldName} занадто коротке (мін. {minLength} символів).";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidateEmail(string email, out string error)
    {
        if (!Regex.IsMatch(email, EmailRegex))
        {
            error = "Невірний формат email.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidateDate(DateTime date, out string error)
    {
        if (date < DateTime.Now)
        {
            error = "Дата повинна бути в майбутньому.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool ValidatePlayers(int min, int max, out string error)
    {
        if (min <= 0 || max <= 0)
        {
            error = "Кількість гравців повинна бути позитивною.";
            return false;
        }
        if (min > max)
        {
            error = "Мінімальна кількість гравців не може перевищувати максимальну.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    public static bool CheckMemberExists(int memberId)
    {
        return DataStore.Members.Any(m => m.Id == memberId);
    }

    public static bool CheckGameExists(int gameId)
    {
        return DataStore.Games.Any(g => g.Id == gameId);
    }

    public static bool CheckSession(int gameId, int memberId, out string error)
    {
        if (!CheckGameExists(gameId))
        {
            error = "Гра не існує.";
            return false;
        }
        if (!CheckMemberExists(memberId))
        {
            error = "Член клубу не існує.";
            return false;
        }
        error = string.Empty;
        return true;
    }
}