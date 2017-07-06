﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LSG.GenericCrud.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Detaileds the compare.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalEntity">The original entity.</param>
        /// <param name="modifiedEntity">The modified entity.</param>
        /// <returns></returns>
        public static string DetailedCompare<T>(this T originalEntity, T modifiedEntity)
        {
            Dictionary<String, object> dict = new Dictionary<string, object>();

            foreach (var prop in originalEntity.GetType().GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly))
            {
                if (prop.Name != "Id")
                {
                    var oldValue = prop.GetValue(originalEntity, null);
                    var newValue = prop.GetValue(modifiedEntity, null);
                    if (oldValue == null)
                    {
                        if (newValue != null)
                        {
                            dict[prop.Name] = newValue;
                        }
                    }
                    else
                    {
                        if (!oldValue.Equals(newValue))
                        {
                            dict[prop.Name] = newValue;
                        }
                    }
                }
            }
            var json = JsonConvert.SerializeObject(dict);
            return json;
        }

    }
}