namespace Kitakun.SPinger.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    public static class WpfExtensions
    {
        public static void AddOnUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }

        public static void OnUI<T>(this T src, Action<T> method) =>
            Application.Current.Dispatcher.BeginInvoke(method, src);
    }
}
