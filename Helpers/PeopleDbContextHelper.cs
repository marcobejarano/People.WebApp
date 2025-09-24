using People.WebApp.Data;

namespace People.WebApp.Helpers
{
    public static class PeopleDbContextHelper
    {
        public static bool PersonExists(PeopleDbContext context, string id)
        {
            return (context.People?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
