namespace CatCar.SharedKernel.Guards;

public static class CpfGuard
{
    public static bool Validate(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || 
            cpf.Length != 11 || 
            !long.TryParse(cpf, out _))
             return false;

        cpf = cpf.Trim().Replace(".", "").Replace("-", "");
        if (cpf.All(c => c == cpf[0])) // Check if all characters are the same
            return false;

        // Implement CPF validation logic here
        return true; // Placeholder for actual validation logic
    }
}