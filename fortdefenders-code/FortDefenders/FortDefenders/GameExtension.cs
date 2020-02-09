using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FortDefenders
{
    public static class GameExtension
    {
        /// <summary>
        /// Allows to get a game service of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="game"></param>
        /// <returns></returns>
        public static T GetService<T>(this Game game) where T : class
        {
            T result = game.Services.GetService(typeof(T)) as T;

            if (Object.ReferenceEquals(result, null))
            {
                throw new Exception("There is no such service");
            }

            return result;
        }

        /// <summary>
        /// Regestering drawable component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="isEnabled"></param>
        /// <param name="isVisible"></param>
        public static void RegisterGameComponent<T>(this Game game, out T component, Boolean isEnabled, Boolean isVisible) where T : DrawableGameComponent
        {
            component = Activator.CreateInstance(typeof(T), game) as T;
            game.Components.Add(component);
            component.Enabled = isEnabled;
            component.Visible = isVisible;
        }

        /// <summary>
        /// Regestering game service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        /// <param name="isEnabled"></param>
        public static void RegisterGameService<T>(this Game game, out T service, Boolean isEnabled) where T : GameComponent
        {
            service = Activator.CreateInstance(typeof(T), game) as T;
            game.Components.Add(service);
            service.Enabled = isEnabled;
            game.Services.AddService(typeof(T), service);
        }
    }
}
