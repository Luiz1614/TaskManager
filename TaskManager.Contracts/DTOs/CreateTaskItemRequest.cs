using System.ComponentModel.DataAnnotations;

namespace TaskManager.Contracts.DTOs;

public record CreateTaskItemRequest(
    [Required(ErrorMessage = "Título é obrigatório.")]
    [MinLength(3, ErrorMessage = "Título deve ter no mínimo 3 caracteres.")]
    [MaxLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres.")]
    string Title,
    
    [MaxLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres.")]
    string? Description,
    DateTime? DueDate
);