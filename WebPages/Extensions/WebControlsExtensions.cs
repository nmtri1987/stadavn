using Microsoft.SharePoint;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace RBVH.Stada.Intranet.WebPages.Extensions
{
    /// <summary>
    /// WebControlsExtensions.
    /// </summary>
    public static class WebControlsExtensions
    {
        public static void LoadDataChoiceFieldDropDown(this DropDownList dropDownList, string fieldName, SPListItem spListItem, string blankValueItem = "", string blankTextItem = "")
        {
            dropDownList.Items.Clear();

            SPList spList = spListItem.ParentList;
            SPFieldMultiChoice fieldMultiChoie = spList.Fields.Cast<SPField>().ToList<SPField>().Where(f => f.StaticName.Equals(fieldName)).FirstOrDefault() as SPFieldMultiChoice;
            if (fieldMultiChoie != null)
            {
                var fieldValue = string.Empty;
                if (spListItem[fieldName] != null)
                {
                    fieldValue = spListItem[fieldName].ToString();
                }
                var choices = fieldMultiChoie.Choices;
                foreach(var choice in choices)
                {
                    ListItem item = new ListItem() { Text = choice, Value = choice };
                    if (string.Compare(fieldValue, choice, true) == 0)
                    {
                        item.Selected = true;
                    }
                    dropDownList.Items.Add(item);
                }

                // Not Required
                if (!fieldMultiChoie.Required)
                {
                    ListItem emptyItem = new ListItem() { Text = blankTextItem, Value = blankValueItem };
                    dropDownList.Items.Insert(0, emptyItem);
                }
            }
        }

        public static void LoadDataChoiceFieldCheckboxes(this CheckBoxList checkBoxList, string fieldName, SPListItem spListItem, TextBox otherValueTextBoxControl, string choiceValueSpliter, string otherValue = "Khác/Others")
        {
            checkBoxList.Items.Clear();
            otherValueTextBoxControl.Text = string.Empty;

            SPList spList = spListItem.ParentList;
            SPFieldMultiChoice fieldMultiChoie = spList.Fields.Cast<SPField>().ToList<SPField>().Where(f => f.StaticName.Equals(fieldName)).FirstOrDefault() as SPFieldMultiChoice;
            if (fieldMultiChoie != null)
            {
                var choices = fieldMultiChoie.Choices;
                foreach (var choice in choices)
                {
                    ListItem item = new ListItem() { Text = choice, Value = choice };
                    checkBoxList.Items.Add(item);
                }

                // Check to select item of checkbox list.
                string[] fieldValueArray = null;
                if (spListItem[fieldName] != null)
                {
                    fieldValueArray = spListItem[fieldName].ToString().Split(new string[] { choiceValueSpliter }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (fieldValueArray != null && fieldValueArray.Length > 0)
                {
                    foreach (var fieldValue in fieldValueArray)
                    {
                        ListItem item = checkBoxList.Items.Cast<ListItem>().Where(listItem => (string.Compare(listItem.Value, fieldValue, true) == 0)).FirstOrDefault();
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            otherValueTextBoxControl.Text = fieldValue;
                            ListItem otherListItem = checkBoxList.Items.Cast<ListItem>().Where(listItem => (string.Compare(listItem.Value, otherValue, true) == 0)).FirstOrDefault();
                            if (otherListItem != null)
                            {
                                otherListItem.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        public static void LoadDataChoiceFieldCheckboxes(this CheckBoxList checkBoxList, string fieldName, SPListItem spListItem, string choiceValueSpliter)
        {
            checkBoxList.Items.Clear();

            SPList spList = spListItem.ParentList;
            SPFieldMultiChoice fieldMultiChoie = spList.Fields.Cast<SPField>().ToList<SPField>().Where(f => f.StaticName.Equals(fieldName)).FirstOrDefault() as SPFieldMultiChoice;
            if (fieldMultiChoie != null)
            {

                var choices = fieldMultiChoie.Choices;
                foreach (var choice in choices)
                {
                    ListItem item = new ListItem() { Text = choice, Value = choice };
                    checkBoxList.Items.Add(item);
                }

                // Check to select item of checkbox list.
                string[] fieldValueArray = null;
                if (spListItem[fieldName] != null)
                {
                    fieldValueArray = spListItem[fieldName].ToString().Split(new string[] { choiceValueSpliter }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (fieldValueArray != null && fieldValueArray.Length > 0)
                {
                    foreach (var fieldValue in fieldValueArray)
                    {
                        ListItem item = checkBoxList.Items.Cast<ListItem>().Where(listItem => (string.Compare(listItem.Value, fieldValue, true) == 0)).FirstOrDefault();
                        if (item != null)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
        }
    }
}
