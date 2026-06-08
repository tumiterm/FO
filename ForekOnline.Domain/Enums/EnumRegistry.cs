// <copyright file="EnumRegistry.cs" company="Forek ICT Services">
//     Copyright © Forek ICT Services.
// </copyright>
// Created By:      Itumeleng Oliphant - (on DESKTOP-72504AI)
// Created Date:    19/01/2023 10:09:27 AM
// Purpose:         Defines the Enums class

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

namespace ForekOnline.Domain.Enums
{
    /// <summary>
    /// The list of enums
    /// </summary>
    public class EnumRegistry
    {
        #region Venue
        public enum eVenueType
        {
            [Display(Name = "Lecture Hall")]
            LectureHall,
            [Display(Name = "Computer Lab")]
            ComputerLab,
            [Display(Name = "Workshop")]
            Workshop,
            [Display(Name = "Examination Hall")]
            ExaminationHall,
            [Display(Name = "Classroom")]
            Boardroom,
            [Display(Name = "Training Room")]
            TrainingRoom,
            [Display(Name = "Open Space")]
            OpenSpace,
            Other
        }

        public enum eVenueStatus
        {
            Active,
            Inactive,
            [Display(Name = "Under Maintenance")]
            UnderMaintenance
        }

        public enum eReservationStatus
        {
            [Display(Name = "Pending HOD Approval")]
            PendingHodApproval,
            Approved,
            Rejected,
            Expired,
            Cancelled,
            Booked
        }

        public enum eReservationAction
        {
            Created,
            Approved,
            Rejected,
            [Display(Name = "Request Changes")]
            RequestChanges,
            Expired,
            Cancelled,
            [Display(Name = "Assessment Booked")]
            AssessmentBooked
        }
        #endregion

        public enum eAdmissionCategory
        {
            FullTime, PartTime, Distance, Online
        }

        /// <summary>
        /// Indicates the compliance status of a user's report submissions.
        /// </summary>
        public enum eComplianceStatus
        {
            /// <summary>All reports submitted on time.</summary>
            [Display(Name = "Fully Compliant")]
            Compliant,

            /// <summary>Has late reports but within acceptable threshold.</summary>
            [Display(Name = "Partially Compliant")]
            PartiallyCompliant,

            /// <summary>Exceeded the late report threshold.</summary>
            [Display(Name = "Non-Compliant")]
            NonCompliant
        }

        public enum eSubscriptionStatus
        {
            Active,
            Expired,
            Suspended,
            Cancelled,
            Trial
        }

        /// <summary>
        /// Represents the type of a resource.
        /// </summary>
        public enum eResourceType
        {
            /// <summary>
            /// Resource is stored in the configured file storage provider (IFileUploadService).
            /// </summary>
            File = 0,

            /// <summary>
            /// Resource is a link to an external system (YouTube, Google Drive, website, etc.)
            /// </summary>
            ExternalLink = 1
        }
        public enum eTypeFile
        {
            Payslip,
            IRP5
        }
        public enum eDocumentType
        {
            [Display(Name = "Knowledge Module")]
            KM,
            [Display(Name = "Practical Module")]
            PM,
            [Display(Name = "Workplace Module")]
            WM,
            [Display(Name = "Student File")]
            File,
        }
        public enum eFileType
        {
            [Display(Name = "Training Schedule")]
            TrainingSchedule,
            Timetable,
            [Display(Name = "Assessment Plan")]
            AssessmentPlan,
            [Display(Name = "Study Material")]
            StudyMaterial,
            [Display(Name = "Attendance Registers")]
            Attendance,
            [Display(Name = "Acknowledgement Registers")]
            Acknowledgement,
            [Display(Name = "Document/Record Request Form")]
            Document
        }
        public enum ApplicationStatus
        {
            Submitted,
            Pending,
            Approved,
            Rejected,
            [Display(Name = "New Submitted")]
            NewSubmitted,
            [Display(Name = "Aptitude Test")]
            AptituteTest
        }
        public enum eStudentDocumentType
        {
            NationalID = 1,
            Passport = 2,
            HighestQualification = 3,

            StudyPermit = 4,   // foreign students only
            GuardianID = 5,   // minors

            ProofOfResidence = 6,
            BirthCertificate = 7,
            MedicalCertificate = 8,
            DisabilityDocument = 9,   // links to HasDisability on Student
            TransferLetter = 10,  // if transferring from another institution
            Photo = 11,  // student profile photo
        }

        /// <summary>
        /// Enum representing different types of emails.
        /// </summary>
        public enum EmailType
        {
            Rejection,
            Approval,
            Aptitude,
            ShortSkillsTradeTest,
            TradesAndNonTrades,
            GenericApproval,
            Pending
        }
        public enum eLicenseFrequency
        {
            [Display(Name = "First Time")]
            First,
            [Display(Name = "Second Time")]
            Second,
            [Display(Name = "Third Time")]
            Third
        }
        public enum eClientType
        {
            Corporate = 12,
            Private = 13,
        }
        public enum HighestQualification
        {
            [Display(Name = "Grade 9")]
            Grade9,
            [Display(Name = "Grade 10")]
            Grade10,
            [Display(Name = "Grade 11")]
            Grade11,
            [Display(Name = "Grade 12")]
            Grade12,
            N1, N2, N3, N4, N5, N6,
            [Display(Name = "Higher Certificate")]
            HigherCertificate,
            Diploma,
            Degree,
            Honours,
            Masters,
            PhD

        }
        public enum eRelationship
        {
            Mother, Father, Uncle, Sister, Brother, Grandfather, Grandmother,
            Husband, Wife, Spouse, Aunt, Other
        }
        public enum eTitle
        {
            Adv, Capt, Dr, Ds, Miss, Ms, Mr, Prof, Rev
        }
        public enum eGender
        {
            Male, Female, Other
        }
        public enum eLearnerAdministration
        {
            [Display(Name = "Learner RSA ID")]
            LearnerID,
            [Display(Name = "Learner Passport")]
            LearnerPassport,
            [Display(Name = "Curriculum Vitae")]
            CV,
            [Display(Name = "Matric Certificate")]
            Matric,
            [Display(Name = "Learner Qualification")]
            Qualification,
            [Display(Name = "Study Permit")]
            StudyPermit,
            [Display(Name = "Parent/Guardian ID")]
            GuardianId,
            [Display(Name = "Proof of Residence")]
            Residence
        }
        public enum eTrainingAdministration
        {
            [Display(Name = "Enrollment Form")]
            EnrollmentForm,
            [Display(Name = "Induction Form")]
            InductionForm,
            [Display(Name = "Declaration of Authenticity")]
            Declaration,
            [Display(Name = "Code of Conduct")]
            Conduct,
            [Display(Name = "Learner Agreement")]
            Agreement,
            [Display(Name = "Training Schedule")]
            Schedule,
            Timetable,
            [Display(Name = "Learner Contract")]
            Contract

        }
        public enum eAssessmentAdministration
        {
            [Display(Name = "Formative Assessment")]
            Formative,
            [Display(Name = "Summative Assessment")]
            Summative,
            [Display(Name = "Practical Assessment")]
            Practical,
            IISA,
            Report,
            [Display(Name = "Formative 1")]
            Formative1,
            [Display(Name = "Formative 2")]
            Formative2,
            [Display(Name = "Formative 3")]
            Formative3,
        }
        public enum eAssessmentType
        {
            Informal = 1,
            Formal = 2
        }
        public enum eAssessmentQuestionType
        {
            MultipleChoice = 1,
            ShortAnswer = 2,
            MathInput = 3
        }
        public enum eAssessmentAttemptStatus
        {
            InProgress = 1,
            Submitted = 2,
            AutoSubmitted = 3,
            Aborted = 4,
            Graded = 5
        }
        public enum eSysRole
        {
            None, Admin, Facilitator, Student, SuperAdmin
        }
        public enum eChoice
        {
            Yes, No
        }
        public enum eCategory
        {
            [Display(Name = "Asylum Seeker")]
            AsylumSeeker,
            LSP,
            [Display(Name = "Permanent Residence Permit")]
            PermanentResidencePermit,
            [Display(Name = "Study Visa")]
            StudyVisa,
            [Display(Name = "Zimbabwe Exception Permit")]
            ZimbabweExceptionPermit,
            [Display(Name = "Not Applicable")]
            NA

        }
        public enum eNType
        {
            N1, N2, N3, N4, N5, N6
        }
        public enum eOperationType
        {
            Course,
            Module,
            Company
        }
        public enum eStatus
        {
            [Display(Name = "Starting Soon")]
            StartingSoon,
            Started,
            [Display(Name = "Dropped Out")]
            DroppedOut,
            Transferred,
            Completed
        }
        public enum eProvince
        {
            Mpumalanga,
            Limpopo,
            [Display(Name = "Northern Cape")]
            NorthernCape,
            [Display(Name = "Western Cape")]
            WesternCape,
            [Display(Name = "Eastern Cape")]
            EasternCape,
            [Display(Name = "Free State")]
            FreeState,
            Gauteng,
            [Display(Name = "KwaZulu-Natal")]
            KwaZuluNatal,
            [Display(Name = "North West")]
            NorthWest
        }
        public enum eSpeciality
        {
            [Display(Name = "Technology Development")]
            TechnologyDevelopment,

            [Display(Name = "E-commerce")]
            ECommerce,

            [Display(Name = "Financial Services")]
            FinancialServices,

            Manufacturing,

            Healthcare,

            [Display(Name = "Energy and Utilities")]
            EnergyUtilities,

            [Display(Name = "Transportation and Logistics")]
            TransportationLogistics,

            [Display(Name = "Consulting and Professional Services")]
            ConsultingServices,

            [Display(Name = "Hospitality and Tourism")]
            HospitalityTourism,

            [Display(Name = "Media and Entertainment")]
            MediaEntertainment,

            [Display(Name = "Real Estate and Construction")]
            RealEstateConstruction,

            [Display(Name = "Retail and Consumer Goods")]
            RetailConsumerGoods,

            Automotive,

            [Display(Name = "Aerospace and Defense")]
            AerospaceDefense,

            [Display(Name = "Education and E-learning")]
            EducationElearning,

            [Display(Name = "Agriculture and Food Production")]
            AgricultureFoodProduction,

            [Display(Name = "Environmental and Sustainability")]
            EnvironmentalSustainability,

            [Display(Name = "Social Impact and Non-profit")]
            SocialImpactNonProfit,

            [Display(Name = "Biotechnology and Pharmaceuticals")]
            BiotechnologyPharmaceuticals,

            [Display(Name = "Art and Culture")]
            ArtCulture,
            Electrical, Welding, Construction,
            [Display(Name = "Occupation Health Management")]
            OHS,
            [Display(Name = "Project Adminstration and Management")]
            ProjectManagement,
            [Display(Name = "Contact Centre Manager")]
            ContactCentreManager,
            [Display(Name = "Office Administration and Management")]
            OfficeAdministrator,
            [Display(Name = "Public Relations")]
            PublicRelations,
            Marketing
        }
        public enum eModule
        {
            [Display(Name = "Module 1")]
            Module1,
            [Display(Name = "Module 2")]
            Module2,
            [Display(Name = "Module 3")]
            Module3,
            [Display(Name = "Module 4")]
            Module4,
            [Display(Name = "Module 5")]
            Module5,
            [Display(Name = "Module 6")]
            Module6,
            [Display(Name = "Module 7")]
            Module7,
            [Display(Name = "Module 8")]
            Module8,
            [Display(Name = "Module 9")]
            Module9,
            [Display(Name = "Module 10")]
            Module10,
            [Display(Name = "Module 11")]
            Module11,
            [Display(Name = "Module 12")]
            Module12,
            [Display(Name = "Module 13")]
            Module13,
            [Display(Name = "Module 14")]
            Module14,
            [Display(Name = "Module 15")]
            Module15,
            [Display(Name = "Test 1")]
            Test1,
            [Display(Name = "Test 2")]
            Test2

        }
        public enum eMaterialType
        {
            Assessment,
            [Display(Name = "Learner Material")]
            Material,
            [Display(Name = "Non Academic")]
            NonAcademic,
            Scope,
            [Display(Name = "Past Exam Question Papers")]
            PastPapers,
            Memorandum, Communique
        }
        public enum eTrade
        {
            Welder, Plumber, Electrician, Bricklayer, Painter,
            OHS,
            [Display(Name = "Office Administrator")]
            OfficeAdmin, NATED,
            [Display(Name = "Pest Management Officer")]
            PestManagement,
            Bookkeeper, ECD,
            [Display(Name = "Computer Technician")]
            ComputerTechnician,
            [Display(Name = "Supply Chain Practitioner")]
            SupplyChain,
            [Display(Name = "Project Management")]
            ProjectManagement
        }
        public enum eDepartment
        {
            None, Welding, Plumbing, Electrical,
            ICT, Engineering, ETQA, Training, Corporate,
            Finance,
            [Display(Name = "Health & Safety")]
            Health,
            Admin, Operations,
            [Display(Name = "Environmental Services")]
            Environmental,
            Marketing

        }
        public enum ReportType
        {
            [Display(Name = "Weekly report")]
            Weekly,
            [Display(Name = "Monthly report")]
            Monthly,
            [Display(Name = "Annual report")]
            Annual
        }
        public enum ePhase
        {
            [Display(Name = "Phase 1")]
            Phase1,
            [Display(Name = "Phase 2")]
            Phase2,
            [Display(Name = "Phase 3")]
            Phase3,
            [Display(Name = "Level 1")]
            Level1,
            [Display(Name = "Level 2")]
            Level2,
            [Display(Name = "Level 3")]
            Level3,
            [Display(Name = "Level 4")]
            Level4,
            ARPL,
            [Display(Name = "Not Applicable")]
            N_A
        }
        public enum eUrgency
        {
            High,
            Moderate,
            Low

        }
        public enum eOperation
        {
            [Display(Name = "I'd like to - Create My Report")]
            Create,
            [Display(Name = "I'd like to - Upload My Report")]
            Upload
        }
        public enum eCourseType
        {
            [Display(Name = "Occupational - Trade")]
            OccupationalTrade,
            [Display(Name = "Occupational - Non Trade")]
            OccupationalNonTrade,
            [Display(Name = "Short Skills")]
            ShortSkills,
            [Display(Name = "Nated Studies")]
            Nated,
            [Display(Name = "Trade Test")]
            TradeTest,
            [Display(Name = "Artisan Recognition of Prior Learning (ARPL)")]
            ARPL,
            [Display(Name = "Foundational Learning Competence (FLC)")]
            FLC,
            Learnership
        }
        public enum eNQF
        {
            [Display(Name = "NQF Level 1")]
            NQFLevel1,
            [Display(Name = "NQF Level 2")]
            NQFLevel2,
            [Display(Name = "NQF Level 3")]
            NQFLevel3,
            [Display(Name = "NQF Level 4")]
            NQFLevel4,
            [Display(Name = "NQF Level 5")]
            NQFLevel5,
            [Display(Name = "NQF Level 6")]
            NQFLevel6,
            [Display(Name = "NQF Level 7")]
            NQFLevel7
        }
        public enum eDuration
        {
            [Display(Name = "One Week")]
            OneWeek,
            [Display(Name = "Two Weeks")]
            TwoWeeks,
            [Display(Name = "Three Weeks")]
            ThreeWeeks,
            [Display(Name = "One Month")]
            Month
        }
        public enum eDurationType
        {
            Day,
            Week,
            Month,
            Year
        }
        public enum eStudyMode
        {
            [Display(Name = "Full Time")]
            FullTime,
            [Display(Name = "Part Time")]
            PartTime,
            [Display(Name = "Self Paced")]
            SelfPaced
        }
        public enum eDeliveryMethod
        {
            Contact,
            Online,
            Hybrid
        }
        public enum eCourseOptionType
        {
            Custom,
            Standard,
            Package
        }
        public enum eCourseChargeType
        {
            Fixed,
            Daily,
            Weekly,
            Monthly
        }
        public enum eFunder
        {
            NSF,
            Mega,
            CETA,
            Silulumanzi,
            Eskom,
            [Display(Name = "Department of Public Works")]
            PublicWorks,
            [Display(Name = "Manganese Metal Company (MMC)")]
            MMC,
            [Display(Name = "Mandlakazi Electrical Technologies")]
            MET,
            EWSETA,
            [Display(Name = "White River Saw Mills")]
            Sawmills,
            [Display(Name = "York Timber")]
            YorkTimber,
            AgriSeta,
            [Display(Name = "Forek Institute of technology")]
            ForekInstitute,
            [Display(Name = "Self-Funded")]
            SelfFunded,
            UIF,
            [Display(Name = "W&RSETA")]
            WRSETA,
            LGSETA


        }
        public enum eFunderType
        {
            [Display(Name = "Self-Funded")]
            SelfFunded,
            [Display(Name = "No Funding")]
            NotFunded,
            [Display(Name = "Company-Funded")]
            Company
        }
        public enum eSelection
        {
            Pending,
            [Display(Name = "Yes - I approve")]
            Yes,
            [Display(Name = "No - I Disapprove")]
            No
        }
        public enum eAttempts
        {
            First, Second, Third
        }
        public enum eFileSelection
        {
            [Display(Name = "Subject File")]
            SubjectFile,
            [Display(Name = "Assessment File")]
            AssessmentFile,
            [Display(Name = "Icass Evidence")]
            IcassEvidence
        }
        public enum eNotificationModalSize
        {
            Small,
            Default,
            Large,
            ExtraLarge
        }
        public enum eNotificationContentType
        {
            Paragraph,
            Html,
            UnorderedList,
            OrderedList,
            Table,
            Image
        }
        public enum eMinimumRequirement
        {
            [Display(Name = "No Requirement")]
            NoRequirement,
            [Display(Name = "Matric")]
            Matric,
            [Display(Name = "3 years of industry experience")]
            Experience,
        }
        public enum eFinancialClearanceStatus
        {
            [Display(Name = "Awaiting Payment")]
            AwaitingPayment,
            [Display(Name = "Proof Uploaded")]
            ProofUploaded,
            Cleared,
            Overridden
        }
    }
}
