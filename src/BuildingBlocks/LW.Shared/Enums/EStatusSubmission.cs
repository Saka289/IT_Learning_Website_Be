using System.ComponentModel.DataAnnotations;

namespace LW.Shared.Enums;

public enum EStatusSubmission
{
    [Display(Name = "In Queue")]
    InQueue = 1,
    [Display(Name = "Processing")]
    Processing = 2,
    [Display(Name = "Accepted")]
    Accepted = 3,
    [Display(Name = "Wrong Answer")]
    WrongAnswer = 4,
    [Display(Name = "Time Limit Exceeded")]
    TimeLimitExceeded = 5,
    [Display(Name = "Compilation Error")]
    CompilationError = 6,
    [Display(Name = "Runtime Error (SIGSEGV)")]
    RuntimeErrorSigsegv = 7,
    [Display(Name = "Runtime Error (SIGXFSZ)")]
    RuntimeErrorSigxfsz = 8,
    [Display(Name = "Runtime Error (SIGFPE)")]
    RuntimeErrorSigfpe = 9,
    [Display(Name = "Runtime Error (SIGABRT)")]
    RuntimeErrorSigabrt = 10,
    [Display(Name = "Runtime Error (NZEC)")]
    RuntimeErrorNzec = 11,
    [Display(Name = "Runtime Error (Other)")]
    RuntimeErrorOther = 12,
    [Display(Name = "Internal Error")]
    InternalError = 13,
    [Display(Name = "Exec Format Error")]
    ExecFormatError = 14,
}