using System.Collections.Generic;
using System.Drawing;
using BaseLib.Param;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;
using PerseusPluginLib.Utils;

namespace PerseusPluginLib.Filter{
	public class FilterCategoricalRow : IMatrixProcessing{
		public bool HasButton { get { return false; } }
		public Bitmap DisplayImage { get { return null; } }
		public string Description { get { return "Those columns are kept or removed that have the specified value in the selected categorical row."; } }
		public string HelpOutput { get { return "The filtered matrix."; } }
		public string[] HelpSupplTables { get { return new string[0]; } }
		public int NumSupplTables { get { return 0; } }
		public string Name { get { return "Filter columns based on categorical row"; } }
		public string Heading { get { return "Filter columns"; } }
		public bool IsActive { get { return true; } }
		public float DisplayRank { get { return 0; } }
		public string[] HelpDocuments { get { return new string[0]; } }
		public int NumDocuments { get { return 0; } }
		public int GetMaxThreads(Parameters parameters) { return 1; }
		public string Url { get { return "http://141.61.102.17/perseus_doku/doku.php?id=perseus:activities:MatrixProcessing:Filtercolumns:FilterCategoricalRow"; } }

		public Parameters GetParameters(IMatrixData mdata, ref string errorString){
			Parameters[] subParams = new Parameters[mdata.CategoryRowCount];
			for (int i = 0; i < mdata.CategoryRowCount; i++){
				string[] values = mdata.GetCategoryRowValuesAt(i);
				int[] sel = values.Length == 1 ? new[]{0} : new int[0];
				subParams[i] =
					new Parameters(new Parameter[]{
						new MultiChoiceParam("Values", sel){
							Values = values,
							Help = "The value that should be present to discard/keep the corresponding row."
						}
					});
			}
			return
				new Parameters(new Parameter[]{
					new SingleChoiceWithSubParams("Row"){
						Values = mdata.CategoryRowNames,
						SubParams = subParams,
						Help = "The categorical row that the filtering should be based on.",
						ParamNameWidth = 50,
						TotalWidth = 731
					},
					new SingleChoiceParam("Mode"){
						Values = new[]{"Remove matching columns", "Keep matching columns"},
						Help =
							"If 'Remove matching columns' is selected, rows having the values specified above will be removed while " +
								"all other rows will be kept. If 'Keep matching columns' is selected, the opposite will happen."
					},
					PerseusPluginUtils.GetFilterModeParam(false)
				});
		}

		public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
			ref IDocumentData[] documents, ProcessInfo processInfo){
			SingleChoiceWithSubParams p = param.GetSingleChoiceWithSubParams("Row");
			int colInd = p.Value;
			if (colInd < 0){
				processInfo.ErrString = "No categorical rows available.";
				return;
			}
			MultiChoiceParam mcp = p.GetSubParameters().GetMultiChoiceParam("Values");
			int[] inds = mcp.Value;
			if (inds.Length == 0){
				processInfo.ErrString = "Please select at least one term for filtering.";
				return;
			}
			string[] values = new string[inds.Length];
			string[] v = mdata.GetCategoryRowValuesAt(colInd);
			for (int i = 0; i < values.Length; i++){
				values[i] = v[inds[i]];
			}
			HashSet<string> value = new HashSet<string>(values);
			bool remove = param.GetSingleChoiceParam("Mode").Value == 0;
			string[][] cats = mdata.GetCategoryRowAt(colInd);
			List<int> valids = new List<int>();
			for (int i = 0; i < cats.Length; i++){
				bool valid = true;
				foreach (string w in cats[i]){
					if (value.Contains(w)){
						valid = false;
						break;
					}
				}
				if ((valid && remove) || (!valid && !remove)){
					valids.Add(i);
				}
			}
			PerseusPluginUtils.FilterColumns(mdata, param, valids.ToArray());
		}
	}
}