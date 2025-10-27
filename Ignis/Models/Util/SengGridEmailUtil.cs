namespace Ignis.Models.Util
{
    public interface ISendGridEmailUtil
    {
        Task SendTemplateEmailAsync(string toEmail, string toName, string templateID, object templateData);
    }
    public class SengGridEmailUtil : ISendGridEmailUtil
    {
        public Task SendTemplateEmailAsync(string toEmail, string toName, string templateID, object templateData)
        {
            throw new NotImplementedException();
        }
    }
}
