using System;
using UnityEngine;

namespace GameTemplate.Runtime.Core.Attributes
{
    /// <summary>
    /// All inspector attributes are available at runtime for reflection but automatically excluded from builds.
    /// This provides a clean API without requiring manual #if UNITY_EDITOR directives.
    /// </summary>
    
    /// <summary>
    /// Forces a field to be shown in the inspector even if it's private.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ShowInInspectorAttribute : PropertyAttribute
    {
        public ShowInInspectorAttribute() { }
    }
    
    /// <summary>
    /// Shows a field only if the specified condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionField { get; }
        public object ConditionValue { get; }
        public bool Invert { get; }
        
        public ShowIfAttribute(string conditionField, object conditionValue = null, bool invert = false)
        {
            ConditionField = conditionField;
            ConditionValue = conditionValue;
            Invert = invert;
        }
    }
    
    /// <summary>
    /// Hides a field only if the specified condition is met.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class HideIfAttribute : PropertyAttribute
    {
        public string ConditionField { get; }
        public object ConditionValue { get; }
        public bool Invert { get; }
        
        public HideIfAttribute(string conditionField, object conditionValue = null, bool invert = false)
        {
            ConditionField = conditionField;
            ConditionValue = conditionValue;
            Invert = invert;
        }
    }
    
    /// <summary>
    /// Makes a field read-only in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyAttribute() { }
    }
    
    /// <summary>
    /// Creates a button in the inspector that calls a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string ButtonText { get; }
        public ButtonStyle Style { get; }
        
        public ButtonAttribute()
        {
            ButtonText = null;
            Style = ButtonStyle.Default;
        }
        
        public ButtonAttribute(string buttonText, ButtonStyle style = ButtonStyle.Default)
        {
            ButtonText = buttonText;
            Style = style;
        }
    }
    
    /// <summary>
    /// Defines the visual style of a button.
    /// </summary>
    public enum ButtonStyle
    {
        Default,
        Primary,
        Secondary,
        Success,
        Warning,
        Danger
    }
    
    /// <summary>
    /// Adds a custom header above a field in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CustomHeaderAttribute : PropertyAttribute
    {
        public string HeaderText { get; }
        public HeaderStyle Style { get; }
        
        public CustomHeaderAttribute(string headerText, HeaderStyle style = HeaderStyle.Default)
        {
            HeaderText = headerText;
            Style = style;
        }
    }
    
    /// <summary>
    /// Defines the visual style of a header.
    /// </summary>
    public enum HeaderStyle
    {
        Default,
        Bold,
        Colored,
        Separator
    }
    
    /// <summary>
    /// Displays an information box in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class InfoBoxAttribute : PropertyAttribute
    {
        public string Message { get; }
        public InfoBoxType Type { get; }
        public string ConditionField { get; }
        public object ConditionValue { get; }
        
        public bool ShowIcon { get; }
        
        public InfoBoxAttribute(string message, InfoBoxType type = InfoBoxType.Default, bool showIcon = true)
        {
            Message = message;
            Type = type;
            ConditionField = null;
            ConditionValue = null;
            ShowIcon = showIcon;
        }
        
        public InfoBoxAttribute(string message, string conditionField, object conditionValue = null, InfoBoxType type = InfoBoxType.Default, bool showIcon = true)
        {
            Message = message;
            Type = type;
            ConditionField = conditionField;
            ConditionValue = conditionValue;
            ShowIcon = showIcon;
        }
    }
    
    /// <summary>
    /// Defines the type of info box.
    /// </summary>
    public enum InfoBoxType
    {
        Default,
        Info,
        Warning,
        Error,
        Success,
    }
    
    /// <summary>
    /// Validates a field value using a method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ValidateInputAttribute : PropertyAttribute
    {
        public string ValidationMethod { get; }
        public string ErrorMessage { get; }
        public ValidationType Type { get; }
        
        public ValidateInputAttribute(string validationMethod, string errorMessage = "Invalid value", ValidationType type = ValidationType.Error)
        {
            ValidationMethod = validationMethod;
            ErrorMessage = errorMessage;
            Type = type;
        }
    }
    
    /// <summary>
    /// Defines the type of validation.
    /// </summary>
    public enum ValidationType
    {
        Error,
        Warning,
        Info
    }
    
    /// <summary>
    /// Adds a range slider to a field in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyRangeAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public string MinField { get; }
        public string MaxField { get; }
        
        public PropertyRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
            MinField = null;
            MaxField = null;
        }
        
        public PropertyRangeAttribute(string minField, string maxField)
        {
            Min = 0f;
            Max = 1f;
            MinField = minField;
            MaxField = maxField;
        }
    }
    
    /// <summary>
    /// Controls the order of properties in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class PropertyOrderAttribute : PropertyAttribute
    {
        public int Order { get; }
        
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }
    }
    
    /// <summary>
    /// Adds spacing in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class PropertySpaceAttribute : PropertyAttribute
    {
        public float Space { get; }
        
        public PropertySpaceAttribute(float space = 10f)
        {
            Space = space;
        }
    }
} 