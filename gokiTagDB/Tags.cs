﻿using GokiLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gokiTagDB
{
    public class Tags
    {
        public static Dictionary<string, string> tags = new Dictionary<string, string>();
        public static Dictionary<string, Color> categories = new Dictionary<string, Color>();
        public static List<List<string>> aliasGroups = new List<List<string>>();

        public static void addTag(string tag, string category)
        {
            tag = tag.ToLower();
            if ( tag.Length > 0 && !tags.ContainsKey(tag))
            {
                if ( category == null || category.Length == 0)
                {
                    category = "default";
                }
                else
                {
                    category = category.ToLower();
                }

                tags.Add(tag, category);
                if ( !categories.ContainsKey(category))
                {
                    categories.Add(category, Color.Black);
                }
            }
        }

        public static void removeTag(string tag)
        {
            if ( tags.ContainsKey(tag))
            {
                tags.Remove(tag);
            }
        }

        public static void addCategory(string category, Color color)
        {
            category = category.ToLower();
            if ( !categories.ContainsKey(category))
            {
                categories.Add(category, color);
            }
        }

        public static void removeCategory(string category)
        {
            if ( categories.ContainsKey(category))
            {
                categories.Remove(category);
            }
        }

        public static void changeCategoryColor(string category, Color color)
        {
            if ( categories.ContainsKey(category))
            {
                categories[category] = color;
            }
        }

        public static byte[] toByteArray()
        {
            AutoSizeGokiBytesWriter writer = new AutoSizeGokiBytesWriter();
            writer.write(tags.Count);
            foreach( KeyValuePair<string,string> tag in tags)
            {
                writer.write(tag.Key);
                writer.write(tag.Value);
            }
            writer.write(categories.Count);
            foreach( KeyValuePair<string, Color> category in categories)
            {
                writer.write(category.Key);
                writer.write(category.Value);
            }
            return writer.Data;
        }

        public static void loadFromByteArray( byte[] data )
        {
            GokiBytesReader reader = new GokiBytesReader(data);
            Tags.tags.Clear();
            Tags.categories.Clear();
            int tags = reader.readInt();
            for( int i = 0; i < tags; i++ )
            {
                Tags.addTag(reader.readString(), reader.readString());
            }
            int categories = reader.readInt();
            for ( int i = 0; i < categories; i++ )
            {
                Tags.addCategory(reader.readString(), reader.readColor());
            }
        }

        public  static void addAliasGroup( List<string> group )
        {
            aliasGroups.Add(group);
        }
    }

}
