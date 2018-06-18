using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Report.RecruitmentModule.Constants;
using System;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.Report.RecruitmentModule
{
    /// <summary>
    /// RecruitmentFormInfo
    /// </summary>
    public class RecruitmentFormInfo
    {
        #region Properties
        public string ID { get; set; }

        public string Title { get; set; }

        public string RecruitmentDepartment { get; set; }

        public string Position { get; set; }

        public string Quantity { get; set; }

        public string ReasonsForRecruitment { get; set; }

        public bool Sex_Male { get; set; }

        public bool Sex_Female { get; set; }

        public string FromAge { get; set; }

        public string ToAge { get; set; }

        public string AvailableTime { get; set; }

        public bool MaritalStatus_Single { get; set; }

        public bool MaritalStatus_Married { get; set; }

        public bool WorkingTime_FullTime { get; set; }

        public bool WorkingTime_WorkInShift { get; set; }

        public bool EducationLevel_HighShool { get; set; }

        public bool EducationLevel_MiddleLevelSchool { get; set; }

        public bool EducationLevel_College { get; set; }

        public bool EducationLevel_University { get; set; }

        public bool EducationLevel_Postgraduate { get; set; }

        public bool Appearance_Necessary { get; set; }

        public bool Appearance_Uncertain { get; set; }

        public bool Appearance_Unnecessary { get; set; }

        public bool WorkingExperience_Under1Year { get; set; }

        public bool WorkingExperience_From1To2Years { get; set; }

        public bool WorkingExperience_Others { get; set; }

        public bool WorkingExperience_Unnecessary { get; set; }

        public string Specialities { get; set; }

        public string DescriptionOfBasicWork { get; set; }

        public string MoralVocations { get; set; }

        public string WorkingAbilities { get; set; }

        // Language Skills
        public bool ComputerSkills_Word { get; set; }

        public bool ComputerSkills_PowerPoint { get; set; }

        public bool ComputerSkills_Excel { get; set; }

        public bool ComputerSkills_Internet { get; set; }

        public bool ComputerSkills_Others { get; set; }

        public string ComputerSkills_OthersText { get; set; }

        public bool OtherSkills_Creative { get; set; }

        public bool OtherSkills_AbilityToLead { get; set; }

        public bool OtherSkills_AbilityToSolveProblem { get; set; }

        public bool OtherSkills_AbilityToMakeDecision { get; set; }

        public bool OtherSkills_AbilityToPersuade { get; set; }

        public bool OtherSkills_Comprehensive { get; set; }

        public bool OtherRequirement_Health { get; set; }

        public bool OtherRequirement_Communication { get; set; }

        public string RequestedDate { get; set; }

        public string RequestedBy { get; set; }

        #endregion

        #region Constructor
        public RecruitmentFormInfo(SPListItem recruitmentItem)
        {
            try
            {
                if (recruitmentItem != null)
                {
                    // ID
                    this.ID = recruitmentItem.ID.ToString();
                    // Title
                    this.Title = recruitmentItem.Title;
                    // Recruitment Department
                    string recruitmentDepartment = string.Empty;
                    if (recruitmentItem[EmployeeRequirementSheetsList.Fields.RecruitmentDepartment] != null)
                    {
                        SPFieldLookupValue recruitmentDepartmentLookupValue = ObjectHelper.GetSPFieldLookupValue(recruitmentItem[EmployeeRequirementSheetsList.Fields.RecruitmentDepartment]);
                        DepartmentDAL departmentDAL = new DepartmentDAL(recruitmentItem.ParentList.ParentWeb.Url);
                        var department = departmentDAL.GetByID(recruitmentDepartmentLookupValue.LookupId);
                        if (department != null)
                        {
                            recruitmentDepartment = string.Format("{0}/{1}", department.VietnameseName, department.Name);
                        }
                    }
                    this.RecruitmentDepartment = recruitmentDepartment;
                    // Position
                    this.Position = ObjectHelper.GetString(recruitmentItem[EmployeeRequirementSheetsList.Fields.Position]);
                    // Quantity
                    this.Quantity = recruitmentItem[EmployeeRequirementSheetsList.Fields.Quantity] != null ? recruitmentItem[EmployeeRequirementSheetsList.Fields.Quantity].ToString() : string.Empty;
                    // Reasons for recuitment
                    this.ReasonsForRecruitment = recruitmentItem[EmployeeRequirementSheetsList.Fields.ReasonsForRecruitment] != null ? recruitmentItem[EmployeeRequirementSheetsList.Fields.ReasonsForRecruitment].ToString() : string.Empty;
                    
                    #region Sex
                    var sexChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.Sex], RecruitmentTemplate.ChoiceValueSpliter);
                    if (sexChoiceValues != null && sexChoiceValues.Length > 0)
                    {
                        this.Sex_Male = sexChoiceValues.ContainsIgnoreCase(SexValues.Male);
                        this.Sex_Female = sexChoiceValues.ContainsIgnoreCase(SexValues.Female);
                    }
                    #endregion

                    // FromAge
                    string fromAge = ObjectHelper.GetString(recruitmentItem[EmployeeRequirementSheetsList.Fields.FromAge]);
                    this.FromAge = string.Compare(fromAge, "0", true) != 0 ? fromAge : string.Empty;
                    // ToAge
                    string toAge = ObjectHelper.GetString(recruitmentItem[EmployeeRequirementSheetsList.Fields.ToAge]);
                    this.ToAge = string.Compare(toAge, "0", true) != 0 ? toAge : string.Empty; ;

                    #region Marital Status
                    var maritalStatusChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.MaritalStatus], RecruitmentTemplate.ChoiceValueSpliter);
                    if (maritalStatusChoiceValues != null && maritalStatusChoiceValues.Length>0)
                    {
                        this.MaritalStatus_Single = maritalStatusChoiceValues.ContainsIgnoreCase(MaritalStatusValues.Single);
                        this.MaritalStatus_Married = maritalStatusChoiceValues.ContainsIgnoreCase(MaritalStatusValues.Married);
                    }
                    #endregion

                    // Available Time
                    this.AvailableTime = recruitmentItem[EmployeeRequirementSheetsList.Fields.AvailableTime] != null ? 
                        (Convert.ToDateTime(recruitmentItem[EmployeeRequirementSheetsList.Fields.AvailableTime])).ToString(StringConstant.DateFormatMMddyyyySlash) : string.Empty;

                    #region Working Time
                    var workingTimeChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.WorkingTime], RecruitmentTemplate.ChoiceValueSpliter);
                    if (workingTimeChoiceValues != null && workingTimeChoiceValues.Length>0)
                    {
                        this.WorkingTime_FullTime = workingTimeChoiceValues.ContainsIgnoreCase(WorkingTimeValues.FullTime);
                        this.WorkingTime_WorkInShift = workingTimeChoiceValues.ContainsIgnoreCase(WorkingTimeValues.WorkInShift);
                    }
                    #endregion

                    #region Education Level
                    var educationLevalChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.EducationLevel], RecruitmentTemplate.ChoiceValueSpliter);
                    if (educationLevalChoiceValues != null && educationLevalChoiceValues.Length > 0)
                    {
                        this.EducationLevel_HighShool = educationLevalChoiceValues.ContainsIgnoreCase(EducationLevelValues.HighSchool);
                        this.EducationLevel_MiddleLevelSchool = educationLevalChoiceValues.ContainsIgnoreCase(EducationLevelValues.MiddleLevelSchool);
                        this.EducationLevel_College = educationLevalChoiceValues.ContainsIgnoreCase(EducationLevelValues.College);
                        this.EducationLevel_University = educationLevalChoiceValues.ContainsIgnoreCase(EducationLevelValues.University);
                        this.EducationLevel_Postgraduate = educationLevalChoiceValues.ContainsIgnoreCase(EducationLevelValues.Postgraduate);
                    }
                    #endregion

                    #region Appearance
                    var appearanceChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.Appearance], RecruitmentTemplate.ChoiceValueSpliter);
                    if (appearanceChoiceValues != null && appearanceChoiceValues.Length > 0)
                    {
                        this.Appearance_Necessary = appearanceChoiceValues.ContainsIgnoreCase(AppearanceValues.Necessary);
                        this.Appearance_Uncertain = appearanceChoiceValues.ContainsIgnoreCase(AppearanceValues.Uncertain);
                        this.Appearance_Unnecessary = appearanceChoiceValues.ContainsIgnoreCase(AppearanceValues.Unnecessary);
                    }
                    #endregion

                    #region Working Experience
                    var workingExperienceChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.WorkingExperience], RecruitmentTemplate.ChoiceValueSpliter);
                    if (workingExperienceChoiceValues != null && workingExperienceChoiceValues.Length>0)
                    {
                        this.WorkingExperience_Under1Year = workingExperienceChoiceValues.ContainsIgnoreCase(WorkingExperienceValues.Under1Year);
                        this.WorkingExperience_From1To2Years = workingExperienceChoiceValues.ContainsIgnoreCase(WorkingExperienceValues.From1To2Years);
                        this.WorkingExperience_Unnecessary = workingExperienceChoiceValues.ContainsIgnoreCase(WorkingExperienceValues.Unnecessary);
                        this.WorkingExperience_Others = workingExperienceChoiceValues.ContainsIgnoreCase(WorkingExperienceValues.Others);
                    }
                    #endregion

                    // Specialities
                    this.Specialities = recruitmentItem[EmployeeRequirementSheetsList.Fields.Specialities] != null ? recruitmentItem[EmployeeRequirementSheetsList.Fields.Specialities].ToString() : string.Empty;
                    // Description of basic work
                    this.DescriptionOfBasicWork = recruitmentItem[EmployeeRequirementSheetsList.Fields.DescriptionOfBasicWork] != null ? recruitmentItem[EmployeeRequirementSheetsList.Fields.DescriptionOfBasicWork].ToString() : string.Empty;
                    // Moral vocations
                    this.MoralVocations = recruitmentItem[EmployeeRequirementSheetsList.Fields.MoralVocations] != null ? recruitmentItem[EmployeeRequirementSheetsList.Fields.MoralVocations].ToString() : string.Empty;
                    // Working abilities
                    this.WorkingAbilities = recruitmentItem[EmployeeRequirementSheetsList.Fields.WorkingAbilities] != null ? recruitmentItem[EmployeeRequirementSheetsList.Fields.WorkingAbilities].ToString() : string.Empty;

                    #region  Computer skills
                    var computerSkillChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.ComputerSkills], RecruitmentTemplate.ChoiceValueSpliter);
                    if (computerSkillChoiceValues != null && computerSkillChoiceValues.Length > 0)
                    {
                        this.ComputerSkills_Word = computerSkillChoiceValues.ContainsIgnoreCase(ComputerSkillValues.Word);
                        this.ComputerSkills_PowerPoint = computerSkillChoiceValues.ContainsIgnoreCase(ComputerSkillValues.PowerPoint);
                        this.ComputerSkills_Excel = computerSkillChoiceValues.ContainsIgnoreCase(ComputerSkillValues.Excel);
                        this.ComputerSkills_Internet = computerSkillChoiceValues.ContainsIgnoreCase(ComputerSkillValues.Internet);
                        this.ComputerSkills_Others = computerSkillChoiceValues.ContainsIgnoreCase(ComputerSkillValues.Others);
                        if (this.ComputerSkills_Others)
                        {
                            this.ComputerSkills_OthersText = computerSkillChoiceValues.GetValueNotInArrayValues(ComputerSkillValues.AllValues);
                        }
                    }
                    #endregion

                    #region Other Skills
                    var otherSkillChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.OtherSkills], RecruitmentTemplate.ChoiceValueSpliter);
                    if (otherSkillChoiceValues != null && otherSkillChoiceValues.Length>0)
                    {
                        this.OtherSkills_Creative = otherSkillChoiceValues.ContainsIgnoreCase(OtherSkillValues.Creative);
                        this.OtherSkills_AbilityToLead = otherSkillChoiceValues.ContainsIgnoreCase(OtherSkillValues.AbilityToLead);
                        this.OtherSkills_AbilityToSolveProblem = otherSkillChoiceValues.ContainsIgnoreCase(OtherSkillValues.AbilityToSolveProblem);
                        this.OtherSkills_AbilityToMakeDecision = otherSkillChoiceValues.ContainsIgnoreCase(OtherSkillValues.AbilityToMakeDecision);
                        this.OtherSkills_AbilityToPersuade = otherSkillChoiceValues.ContainsIgnoreCase(OtherSkillValues.AbilityToPersuade);
                        this.OtherSkills_Comprehensive = otherSkillChoiceValues.ContainsIgnoreCase(OtherSkillValues.Comprehensive);
                    }
                    #endregion

                    #region Other Requirements
                    var otherRequirementChoiceValues = ObjectHelper.GetChoiceValues(recruitmentItem[EmployeeRequirementSheetsList.Fields.OtherRequirement], RecruitmentTemplate.ChoiceValueSpliter);
                    if (otherRequirementChoiceValues != null && otherRequirementChoiceValues.Length > 0)
                    {
                        this.OtherRequirement_Health = otherRequirementChoiceValues.ContainsIgnoreCase(OtherRequirementValues.Health);
                        this.OtherRequirement_Communication = otherRequirementChoiceValues.ContainsIgnoreCase(OtherRequirementValues.Communication);
                    }
                    #endregion

                    // Requested Date
                    this.RequestedDate = recruitmentItem[StringConstant.DefaultSPListField.CreatedField] != null ?
                        (Convert.ToDateTime(recruitmentItem[StringConstant.DefaultSPListField.CreatedField])).ToString(StringConstant.DateFormatMMddyyyySlash) : string.Empty;

                    // Requested By
                    SPFieldLookupValue creator = ObjectHelper.GetSPFieldLookupValue(recruitmentItem[EmployeeRequirementSheetsList.Fields.CommonCreatorField]);
                    if (creator != null)
                    {
                        this.RequestedBy = creator.LookupValue;
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }
        #endregion
    }
}
