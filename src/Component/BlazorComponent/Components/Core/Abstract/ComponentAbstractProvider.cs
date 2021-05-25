﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorComponent
{
    public class ComponentAbstractProvider
    {
        private readonly Dictionary<ComponentKey, Type> _typeConfig = new();
        private readonly Dictionary<ComponentKey, Action<Dictionary<string, object>>> _propertiesConfig = new();

        public ComponentAbstractProvider Apply<TComponent, TImplementComponent>(Action<Dictionary<string, object>> propertiesAction = null)
            where TImplementComponent : TComponent
        {
            var key = ComponentKey.Get<TComponent>();
            return Apply<TComponent, TImplementComponent>(key, propertiesAction);
        }

        private ComponentAbstractProvider Apply<TComponent, TImplementComponent>(ComponentKey key, Action<Dictionary<string, object>> propertiesAction) where TImplementComponent : TComponent
        {
            if (_typeConfig.TryAdd(key, typeof(TImplementComponent)))
            {
                _propertiesConfig[key] = propertiesAction;
            }

            return this;
        }

        public ComponentAbstractProvider Apply<TComponent, TImplementComponent>(string name, Action<Dictionary<string, object>> propertiesAction = null)
            where TImplementComponent : TComponent
        {
            var key = ComponentKey.Get<TComponent>(name);
            return Apply<TComponent, TImplementComponent>(key, propertiesAction);
        }

        /// <summary>
        /// Use this with <see cref="GetMetadata(string)"/>
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="name"></param>
        /// <param name="propertiesAction"></param>
        /// <returns></returns>
        public ComponentAbstractProvider Apply<TComponent>(string name, Action<Dictionary<string, object>> propertiesAction = null)
            where TComponent : IComponent
        {
            return Apply<IComponent, TComponent>(name, propertiesAction);
        }

        public ComponentAbstractProvider Merge<TComponent>(Action<Dictionary<string, object>> mergePropertiesAction = null)
        {
            var key = ComponentKey.Get<TComponent>();
            Merge(key, mergePropertiesAction);

            return this;
        }

        private void Merge(ComponentKey key, Action<Dictionary<string, object>> mergePropertiesAction = null)
        {
            if (mergePropertiesAction != null)
            {
                var propertiesAction = _propertiesConfig.GetValueOrDefault(key);
                _propertiesConfig[key] = properties =>
                {
                    propertiesAction?.Invoke(properties);
                    mergePropertiesAction?.Invoke(properties);
                };
            }
        }

        public ComponentAbstractProvider Merge<TComponent>(string name, Action<Dictionary<string, object>> mergePropertiesAction = null)
        {
            var key = ComponentKey.Get<TComponent>(name);
            Merge(key, mergePropertiesAction);

            return this;
        }

        /// <summary>
        /// Use this with <see cref="Apply{TComponent}(string, Action{Dictionary{string, object}})"/> and <see cref="GetMetadata(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mergePropertiesAction"></param>
        /// <returns></returns>
        public ComponentAbstractProvider Merge(string name, Action<Dictionary<string, object>> mergePropertiesAction = null)
        {
            var key = ComponentKey.Get<IComponent>(name);
            Merge(key, mergePropertiesAction);

            return this;
        }

        public AbstractMetadata GetMetadata<TComponent>()
        {
            var type = _typeConfig.GetValueOrDefault(ComponentKey.Get<TComponent>(), typeof(TComponent));

            var properties = new Dictionary<string, object>();
            var action = _propertiesConfig.GetValueOrDefault(ComponentKey.Get<TComponent>());
            action?.Invoke(properties);

            return new AbstractMetadata(type, properties);
        }

        public AbstractMetadata GetMetadata<TComponent>(string name)
        {
            var type = _typeConfig.GetValueOrDefault(ComponentKey.Get<TComponent>(name), typeof(TComponent));

            var properties = new Dictionary<string, object>();
            var action = _propertiesConfig.GetValueOrDefault(ComponentKey.Get<TComponent>(name));
            action?.Invoke(properties);

            return new AbstractMetadata(type, properties);
        }

        /// <summary>
        /// Use this with <see cref="Apply{TComponent}(string, Action{Dictionary{string, object}})"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AbstractMetadata GetMetadata(string name)
        {
            return GetMetadata<IComponent>(name);
        }
    }
}
