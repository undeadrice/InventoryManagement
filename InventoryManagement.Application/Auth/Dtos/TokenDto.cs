namespace InventoryManagement.Application.Auth.Dtos;

public record TokenDto(string Token, DateTime ValidTo);