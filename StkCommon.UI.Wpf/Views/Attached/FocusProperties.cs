using StkCommon.UI.Wpf.Events;
using System.Windows;
using System.Windows.Input;

namespace StkCommon.UI.Wpf.Views.Attached
{
    /// <summary> a utility tool to save and restore the focused element of in the scope of a
    /// determinate control (container) and change the look & feel with commands (focused and
    /// unfocused) </summary>
    public class FocusProperties
    {
        /// <summary>
        /// the event occurs after the changing of the foucsed element of a focusable container
        /// </summary>
        public static readonly RoutedEvent FocusedElementChangedEvent =
            EventManager.RegisterRoutedEvent("FocusedElementChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FocusProperties));

        /// <summary>
        /// the event occurs before the changing of the foucsed element of a focusable container
        /// </summary>
        public static readonly RoutedEvent FocusedElementChangingEvent =
            EventManager.RegisterRoutedEvent("FocusedElementChanging", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FocusProperties));

        public static void AddFocusedElementChangedHandler(DependencyObject container, RoutedEventHandler handler)
        {
            EventManagerEx.AddHandlerToDependencyObject(container, FocusedElementChangedEvent, handler);
        }

        public static void AddFocusedElementChangingHandler(DependencyObject container, RoutedEventHandler handler)
        {
            EventManagerEx.AddHandlerToDependencyObject(container, FocusedElementChangingEvent, handler);
        }

        public static void RemoveFocusedElementChangedHandler(DependencyObject container, RoutedEventHandler handler)
        {
            EventManagerEx.RemoveHandlerFromDependencyObject(container, FocusedElementChangedEvent, handler);
        }

        public static void RemoveFocusedElementChangingHandler(DependencyObject container, RoutedEventHandler handler)
        {
            EventManagerEx.RemoveHandlerFromDependencyObject(container, FocusedElementChangingEvent, handler);
        }

        /// <summary>
        /// activate a DependencyObject as a container of a set of focusable children, when true. the
        /// focusable children should be setted as IsFocusableElement = true
        /// </summary>
        public static readonly DependencyProperty IsFocusableContainerProperty =
            DependencyProperty.RegisterAttached("IsFocusableContainer", typeof(bool), typeof(FocusProperties),
                                                new PropertyMetadata(false, IsFocusableContainerOnChanged));

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static bool GetIsFocusableContainer(DependencyObject container)
        {
            return (bool)container.GetValue(IsFocusableContainerProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static void SetIsFocusableContainer(DependencyObject container, bool activated)
        {
            container.SetValue(IsFocusableContainerProperty, activated);
        }

        /// <summary>
        /// set the container as FocusScope to guarantee that only one focusable element can get the
        /// logical focus, and only this element can raise the event "GotFocus" to change his looking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IsFocusableContainerOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FocusManager.SetIsFocusScope(sender, (bool)e.NewValue);
        }

        /// <summary>
        /// save the current focused element of a focusable container. as attached property it should
        /// be hooked on the same element (container object) with the focusable container
        /// </summary>
        public static readonly DependencyProperty FocusedElementProperty =
            DependencyProperty.RegisterAttached("FocusedElement", typeof(IInputElement), typeof(FocusProperties),
                                                new PropertyMetadata(null, FocusedElementPropertyOnChanged));

        public static void FocusOnElement(IInputElement oldFocusedElement, IInputElement newFocusedElement)
        {
            if (oldFocusedElement != null)
                ExecuteCommand(GetUnfocusedCommand(oldFocusedElement), oldFocusedElement);
            if (newFocusedElement != null)
                ExecuteCommand(GetFocusedCommand(newFocusedElement), newFocusedElement);
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static IInputElement GetFocusedElement(DependencyObject container)
        {
            return container.GetValue(FocusedElementProperty) as IInputElement;
        }

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static void SetFocusedElement(DependencyObject container, IInputElement iie)
        {
            EventManagerEx.RaiseRoutedEvent(container, FocusedElementChangingEvent);
            container.SetValue(FocusedElementProperty, iie);
            EventManagerEx.RaiseRoutedEvent(container, FocusedElementChangedEvent);
        }

        private static void FocusedElementPropertyOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FocusOnElement(e.OldValue as IInputElement, e.NewValue as IInputElement);
        }

        //decribes the most-upper element which can contain a set of focusable children
        //while the changing of the real focus (through keyboard, mouse etc.) the focused element of this container
        //should be marked and be refund

        /// <summary>
        /// gets or sets the container of a focusable element
        /// </summary>
        public static readonly DependencyProperty FocusableElementContainerProperty =
            DependencyProperty.RegisterAttached("FocusableElementContainer", typeof(DependencyObject), typeof(FocusProperties), new PropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static DependencyObject GetFocusableElementContainer(IInputElement focusedElement)
        {
            var dependencyObject = focusedElement as DependencyObject;
            if (dependencyObject != null)
            {
                return dependencyObject.GetValue(FocusableElementContainerProperty) as DependencyObject;
            }
            return null;
        }

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static void SetFocusableElementContainer(IInputElement focusedElement, DependencyObject container)
        {
            var dependencyObject = focusedElement as DependencyObject;
            if (dependencyObject != null)
            {
                dependencyObject.SetValue(FocusableElementContainerProperty, container);
            }
        }

        /// <summary>
        /// activate a IInputElement as focusable element, when true. it should be used on an element
        /// which is in a focusable container
        /// </summary>
        public static readonly DependencyProperty IsFocusableElementProperty =
            DependencyProperty.RegisterAttached("IsFocusableElement", typeof(bool), typeof(FocusProperties),
                                                new PropertyMetadata(false, IsFocusableElementOnChanged));

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static bool GetIsFocusableElement(IInputElement iie)
        {
            var dependencyObject = iie as DependencyObject;
            if (dependencyObject != null)
            {
                return (bool)dependencyObject.GetValue(IsFocusableElementProperty);
            }
            return false;
        }

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static void SetIsFocusableElement(IInputElement iie, bool activated)
        {
            var dependencyObject = iie as DependencyObject;
            if (dependencyObject != null)
            {
                dependencyObject.SetValue(IsFocusableElementProperty, activated);
            }
        }

        /// <summary>
        /// save the current focused element and change the lookinng for the focused and unfocused elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FocusableElementGotFocus(object sender, RoutedEventArgs e)
        {
            //before setting the focused element must know whether the current element and focused element sind same or not
            var inputElement = sender as IInputElement;
            if (inputElement == null)
                return;
            var container = GetFocusableElementContainer(inputElement);
            var focused = GetFocusedElement(container);
            if (container != null && !inputElement.Equals(focused))
            {
                SetFocusedElement(container, inputElement);
            }
        }

        /// <summary>
        /// when mouse down on the focusable element or a child element of this focusable element a
        /// PreviewMouseDown event would be raised to set the focus on this focusable element and
        /// raises the GotFocus event on it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FocusableElementPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //before capture the focus must know whether the current elemnt and focused element sind same or not
            var iie = sender as IInputElement;
            var container = GetFocusableElementContainer(iie);
            var focused = GetFocusedElement(container);
            if (iie != null && !iie.Equals(focused))
            {
                iie.Focus();
            }
        }

        /// <summary>
        /// a focusable element can handle the "PreviewMouseDown" event to get the focus to raise the
        /// "GotFocus" event, and handle this GotFocus-event to change his looking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IsFocusableElementOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var container = GetContainer(sender as IInputElement);
                SetFocusableElementContainer(sender as IInputElement, container);
                var uie = sender as UIElement;

                if (uie != null)
                {
                    uie.PreviewMouseDown += FocusableElementPreviewMouseDown;
                    uie.GotFocus += FocusableElementGotFocus;
                }
            }
            else
            {
                SetFocusableElementContainer(sender as IInputElement, null);
                var uie = sender as UIElement;
                if (uie != null)
                {
                    uie.PreviewMouseDown -= FocusableElementPreviewMouseDown;
                    uie.GotFocus -= FocusableElementGotFocus;
                }
            }
        }

        /// <summary>
        /// a command to change the looking of the current focused element
        /// </summary>
        public static readonly DependencyProperty FocusedCommandProperty =
            DependencyProperty.RegisterAttached("FocusedCommand", typeof(ICommand), typeof(FocusProperties), new PropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static ICommand GetFocusedCommand(IInputElement focusableElement)
        {
            var dependencyObject = focusableElement as DependencyObject;
            if (dependencyObject != null)
            {
                return dependencyObject.GetValue(FocusedCommandProperty) as ICommand;
            }
            return null;
        }

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static void SetFocusedCommand(IInputElement focusableElement, ICommand command)
        {
            var dependencyObject = focusableElement as DependencyObject;
            if (dependencyObject != null)
            {
                dependencyObject.SetValue(FocusedCommandProperty, command);
            }
        }

        /// <summary>
        /// a command to change the looking of the last focused element (current unfocused)
        /// </summary>
        public static readonly DependencyProperty UnfocusedCommandProperty =
            DependencyProperty.RegisterAttached("UnfocusedCommand", typeof(ICommand), typeof(FocusProperties), new PropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static ICommand GetUnfocusedCommand(IInputElement focusableElement)
        {
            if (focusableElement is DependencyObject)
            {
                return (focusableElement as DependencyObject).GetValue(UnfocusedCommandProperty) as ICommand;
            }
            return null;
        }

        [AttachedPropertyBrowsableForType(typeof(IInputElement))]
        public static void SetUnfocusedCommand(IInputElement focusableElement, ICommand command)
        {
            var dependencyObject = focusableElement as DependencyObject;
            if (dependencyObject != null)
            {
                dependencyObject.SetValue(UnfocusedCommandProperty, command);
            }
        }

        /// <summary>
        /// get the container of a focusable element
        /// </summary>
        /// <param name="iie"></param>
        /// <returns></returns>
        public static DependencyObject GetContainer(IInputElement iie)
        {
            if (iie != null && GetIsFocusableElement(iie))
            {
                if (iie is DependencyObject)
                {
                    var doe = iie as DependencyObject;
                    while (doe != null)
                    {
                        if (GetIsFocusableContainer(doe))
                        {
                            return doe;
                        }
                        if (doe is FrameworkElement)
                        {
                            var parent = (doe as FrameworkElement).Parent ?? (doe as FrameworkElement).TemplatedParent;
                            doe = parent;
                        }
                        else if (doe is FrameworkContentElement)
                        {
                            var parent = (doe as FrameworkContentElement).Parent ?? (doe as FrameworkContentElement).TemplatedParent;
                            doe = parent;
                        }
                        else
                            break;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// execute a command for the given parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        private static void ExecuteCommand(ICommand command, object param)
        {
            if (command != null && command.CanExecute(param))
                command.Execute(param);
        }
    }
}