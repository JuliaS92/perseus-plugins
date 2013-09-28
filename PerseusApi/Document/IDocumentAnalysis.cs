﻿using System;
using BasicLib.Param;
using PerseusApi.Matrix;

namespace PerseusApi.Document {
	public interface IDocumentAnalysis : IDocumentActivity, IAnalysis {
		Tuple<IDocumentProcessing, Func<Parameters, IDocumentData, Parameters, string>>[] Replacements { get; }
		IMatrixAnalysisResult AnalyzeData(IDocumentData mdata, Parameters param, ProcessInfo processInfo);

		/// <summary>
		/// Define here the parameters that determine the specifics of the analysis.
		/// </summary>
		/// <param name="mdata">The parameters might depend on the document.</param>
		/// <param name="errString">Set this to a value != null if an error occured. The error string will be displayed to the user.</param>
		/// <returns>The set of parameters.</returns>
		Parameters GetParameters(IDocumentData mdata, ref string errString);
	}
}
