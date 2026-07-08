namespace FamilyClaimRef.App.Services.Localization;

public interface IUiTextProvider
{
    string Get(string key);

    string Format(string key, params object?[] args);
}
