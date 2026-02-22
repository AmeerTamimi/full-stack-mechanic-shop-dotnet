namespace GOATY.Application.Features.Common.Interfaces
{
    public static class UtilityService
    {
        public static string MaskEmail(string email)
        {
            int idx = email.IndexOf('@');

            return email[0] + "****" + email[idx - 1] + email[idx..];
        }
    }
}
