// Copyright � 2006 Microsoft Corporation.  All Rights Reserved

namespace Snoop
{
	using System.Text.RegularExpressions;
	using System.Windows;

	public class PropertyFilter {
		private string filterString;
		private Regex filterRegex;
		private bool showDefaults;

		public PropertyFilter(string filterString, bool showDefaults) {
			this.filterString = filterString.ToLower();
			this.showDefaults = showDefaults;
		}

		public string FilterString {
			get { return this.filterString; }
			set {
				this.filterString = value.ToLower();
				try
				{
					this.filterRegex = new Regex(this.filterString, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
				}
				catch
				{
					this.filterRegex = null;
				}
			}
		}

		public bool ShowDefaults {
			get { return this.showDefaults; }
			set { this.showDefaults = value; }
		}

		//DHDH - PropertyFilter support
		public PropertyFilterSet SelectedFilterSet { get; set; }

		public bool IsPropertyFilterSet
		{
			get
			{
				return (SelectedFilterSet != null && SelectedFilterSet.Properties != null);
			}
		}

		public bool Show(PropertyInformation property) {
			if (this.filterRegex == null && string.IsNullOrEmpty(this.filterString) && !IsPropertyFilterSet)
			{
				if (!this.ShowDefaults && property.ValueSource.BaseValueSource == BaseValueSource.Default)
					return false;
				return true;
			}

			// Use a regular expression if we have one.
			if (this.filterRegex != null)
			{
				return (this.filterRegex.IsMatch(property.DisplayName) ||
					this.filterRegex.IsMatch(property.Property.PropertyType.Name) ||
					this.filterRegex.IsMatch(property.Property.ComponentType.Name));
			}

			//DHDH - check if filter set is applied
			if (IsPropertyFilterSet)
			{
				if (SelectedFilterSet.IsPropertyInFilter(property.DisplayName))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			// Otherwise, just check for string containment.
			if (property.DisplayName.ToLower().Contains(this.FilterString))
				return true;
			if (property.Property.PropertyType.Name.ToLower().Contains(this.FilterString))
				return true;
			if (property.Property.ComponentType.Name.ToLower().Contains(this.FilterString))
				return true;
			return false;
		}
	}

	public class PropertyFilterSet
	{
		public string DisplayName
		{
			get;
			set;
		}

		public bool IsDefault
		{
			get;
			set;
		}

		public string[] Properties
		{
			get;
			set;
		}

		public bool IsPropertyInFilter(string property)
		{
			string lowerProperty = property.ToLower();
			foreach (var filterProp in Properties)
			{
				if (lowerProperty.Contains(filterProp))
				{
					return true;
				}
			}
			return false;
		}
	}
}