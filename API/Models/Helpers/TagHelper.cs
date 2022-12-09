using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Persistence;

namespace API.Models.Helpers
{
    public class TagHelper
    {
        public static bool IsTagHere(DataContext context, string tag)
        {
            return context.Tags.FirstOrDefault(t => t.Text.Equals(tag)) != null;
        }

        public static List<Tag> AddTagsIfNotPresent(DataContext context, IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                Tag tagInDatabase = context.Tags.FirstOrDefault(t => t.Text.Equals(tag));

                if (tagInDatabase == null)
                {
                    context.Tags.Add(new Tag()
                    {
                        Text = tag
                    });
                }
            }
            context.SaveChanges();

            List<Tag> result = new List<Tag>();
            foreach (var tag in tags)
            {
                Tag t = context.Tags.FirstOrDefault(x => x.Text.Equals(tag));
                result.Add(t);
            }

            return result;
        }
        
    }
}