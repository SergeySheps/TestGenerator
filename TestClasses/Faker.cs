using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Generators;

namespace FakerLibrary
{
    public class Faker
    {
        private Dictionary<Type, IGenerator> generators;
        private List<object> nestedObjects;
        private object reflectionObject;

        public Faker()
        {
            generators = new Dictionary<Type, IGenerator>();
            nestedObjects = new List<object>();
            initializeGenDicitonary();
            GetGenerators("../../../Plugins/bin/Debug/Plugins.dll");
        }

        private void initializeGenDicitonary()
        {
            generators.Add(typeof(String), (IGenerator)Activator.CreateInstance(typeof(StringGenerator)));
            generators.Add(typeof(Int32), (IGenerator)Activator.CreateInstance(typeof(Int32Generator)));
            generators.Add(typeof(Int64), (IGenerator)Activator.CreateInstance(typeof(Int64Generator)));
            generators.Add(typeof(float), (IGenerator)Activator.CreateInstance(typeof(FloatGenerator)));
            generators.Add(typeof(double), (IGenerator)Activator.CreateInstance(typeof(DoubleGenerator)));
            generators.Add(typeof(DateTime), (IGenerator)Activator.CreateInstance(typeof(DateTimeGenerator)));
        }


        private void GetGenerators(string path)
        {
            var asm = Assembly.LoadFrom(path);
            foreach (var type in asm.GetTypes())
            {
                if (type.GetInterface(typeof(IGenerator).FullName) != null)
                {
                    var gen = (IGenerator)Activator.CreateInstance(type);
                    generators.Add(gen.Type, gen);
                }
            }
        }

        public T Create<T>(object nested = null)
        {
            if (nested != null)
            {
                nestedObjects.Add(nested);
            }

            var constructors = typeof(T).GetConstructors().OrderByDescending(x => x.GetParameters().Length);
            ConstructorInfo constructor = null;
            object[] parameters = null;
            try
            {
                constructor = constructors.First();

                var parametersInfo = constructor.GetParameters();
                parameters = new object[parametersInfo.Length];

                for (int i = 0; i < parametersInfo.Length; i++)
                {
                    if (generators.TryGetValue(parametersInfo[i].ParameterType, out var generator))
                    {
                        parameters[i] = generator.GetValue();
                    }
                }
            }
            catch (Exception)
            {

            }

            var result = constructor != null ? constructor.Invoke(parameters) : Activator.CreateInstance(typeof(T));

            foreach (var property in typeof(T).GetProperties())
            {
                if (property?.SetMethod != null && property.SetMethod.IsPublic)
                {
                    SetValue(ref result, property);
                }
            }

            return (T)result;
        }

        private void SetValue<T>(ref T result, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            string objectType = result.GetType().Name;

            if (generators.TryGetValue(propertyType, out var generator))
            {
                property.SetMethod.Invoke(result, new[] { generator.GetValue() });
            }
            else if (propertyType.Name == objectType)
            {
                property.SetMethod.Invoke(result, new object[] { result });
            }
            else if (nestedObjects.Find(x => x.GetType() == propertyType) != null)
            {
                reflectionObject = nestedObjects.Find(x => x.GetType() == propertyType);
                property.SetMethod.Invoke(result, new[] { nestedObjects.Find(x => x.GetType() == propertyType) });
                nestedObjects.Remove(reflectionObject);
            }
            else
            {
                property.SetMethod.Invoke(result, new[] { GetDTO(ref result, propertyType) });
            }
        }

        private object GetDTO<T>(ref T result, Type property)
        {
            try
            {
                var method = typeof(Faker).GetMethod("Create", BindingFlags.Instance | BindingFlags.Public);
                var genericMethod = method?.MakeGenericMethod(property);
                var dto = genericMethod?.Invoke(this, new object[] { result });
                return dto;
            }
            catch (Exception)
            {
                // if unknown dto
            }

            return null;
        }

    }

}
