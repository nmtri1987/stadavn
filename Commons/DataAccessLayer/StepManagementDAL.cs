using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class StepManagementDAL : BaseDAL<StepManagement>
    {
        public StepManagementDAL(string siteUrl) : base(siteUrl)
        {

        }
        public StepManagement GetNextStepManagement(string StepStatus, StepModuleList moduleStep, int departmentId = 0)
        {
            string query = $@" <Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='CurrentStepStatus' />
                                        <Value Type='Choice'>{StepStatus}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='StepModule' />
                                        <Value Type='Choice'>{moduleStep.ToString()}</Value>
                                    </Eq>
                                </And>
                            </Where>";

            List<StepManagement> steps = GetByQuery(query);
            if (steps != null && steps.Count > 0)
            {
                var currentStep = steps.Where(e => e.CommonDepartment != null && e.CommonDepartment.LookupId == departmentId).FirstOrDefault();
                if (currentStep == null)
                {
                    currentStep = steps.Where(e => e.CommonDepartment == null).FirstOrDefault();
                }

                if (currentStep != null)
                {
                    return GetStepManagement(currentStep.StepNumber + 1, moduleStep, departmentId);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public StepManagement GetStepManagement(string StepStatus, StepModuleList moduleStep, int departmentId = 0)
        {
            string query = $@" <Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='CurrentStepStatus' />
                                        <Value Type='Choice'>{StepStatus}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='StepModule' />
                                        <Value Type='Choice'>{moduleStep.ToString()}</Value>
                                    </Eq>
                                </And>
                            </Where>";

            List<StepManagement> steps = GetByQuery(query);

            if (steps != null && steps.Count() > 0)
            {
                var stepManagement = steps.Where(e => e.CommonDepartment != null && e.CommonDepartment.LookupId == departmentId).FirstOrDefault();
                if (stepManagement == null)
                {
                    stepManagement = steps.Where(e => e.CommonDepartment == null).FirstOrDefault();
                }

                return stepManagement;
            }
            else
            {
                return null;
            }
        }

        public StepManagement GetStepManagement(int step, StepModuleList moduleStep, int departmentId = 0)
        {
            string query = $@" <Where>
                                <And>
                                    <Eq>
                                        <FieldRef Name='StepNumber' />
                                        <Value Type='Number'>{step}</Value>
                                    </Eq>
                                    <Eq>
                                        <FieldRef Name='StepModule' />
                                        <Value Type='Choice'>{moduleStep.ToString()}</Value>
                                    </Eq>
                                </And>
                            </Where>";

            List<StepManagement> steps = GetByQuery(query);

            if (steps != null && steps.Count() > 0)
            {
                var stepManagement = steps.Where(e => e.CommonDepartment != null && e.CommonDepartment.LookupId == departmentId).FirstOrDefault();
                if (stepManagement == null)
                {
                    stepManagement = steps.Where(e => e.CommonDepartment == null).FirstOrDefault();
                }

                return stepManagement;
            }
            else
            {
                return null;
            }
        }
    }
}
