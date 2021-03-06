﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Template10.Common;
using Template10.Utils;

namespace Template10.Mvvm
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-MVVM
    public abstract class BindableBase : IBindable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;

            var handler = this.PropertyChanged;
            //if is not null
            if (!object.Equals(handler, null))
            {
                var args = new PropertyChangedEventArgs(propertyName);
                try
                {
                    handler.Invoke(this, args);
                }
                catch
                {
                    WindowWrapper.Current().Dispatcher.Dispatch(() => handler.Invoke(this, args));
                }
            }
        }

        public virtual bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public virtual bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            //if is equal 
            if (object.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            RaisePropertyChanged(propertyExpression);
            return true;
        }

        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                return;

            var handler = PropertyChanged;
            //if is not null
            if (!object.Equals(handler, null))
            {
                var propertyName = ExpressionUtils.GetPropertyName(propertyExpression);

                if (!object.Equals(propertyName, null))
                {
                    var args = new PropertyChangedEventArgs(propertyName);
                    try
                    {
                        handler.Invoke(this, args);
                    }
                    catch
                    {
                        WindowWrapper.Current().Dispatcher.Dispatch(() => handler.Invoke(this, args));
                    }
                }
            }
        }
    }
}
