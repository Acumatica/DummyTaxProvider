using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using PX.Data;

namespace PX.TaxProvider.DummyTaxProvider
{
	public class DummyTaxProvider : ITaxProvider
	{
		public const string TaxProviderID = "DummyTaxProvider";
		private const string ActiveSettingID = "Active";
		private const string WriteTraceSettingID = "WriteTrace";
		private const decimal TaxPercentage = 0.1m;
		public ITaxProviderSetting[] DefaultSettings
		{
			get
			{
				return new ITaxProviderSetting[]
					{   
						new TaxProviderSetting(TaxProviderID, ActiveSettingID,  1, Messages.ActiveSettingDisplaytName,(true).ToString(), TaxProviderSettingControlType.Checkbox),
						new TaxProviderSetting(TaxProviderID, WriteTraceSettingID,  2, Messages.ActiveSettingDisplaytName,(true).ToString(), TaxProviderSettingControlType.Checkbox)
					};
			}
		}

		protected bool IsActive { get; set; }
		protected bool WriteTrace { get; set; }

		public void Initialize(IEnumerable<ITaxProviderSetting> settings)
		{
			foreach (var setting in settings)
			{
				if (ActiveSettingID.Equals(setting.SettingID, StringComparison.InvariantCultureIgnoreCase))
				{
					IsActive = Convert.ToBoolean(setting.Value);
				}
				if (WriteTraceSettingID.Equals(setting.SettingID, StringComparison.InvariantCultureIgnoreCase))
				{
					WriteTrace = Convert.ToBoolean(setting.Value);
				}
			}
		}

		public IReadOnlyList<string> Attributes
		{
			get { return new List<string>().AsReadOnly(); }
		}

		#region Methods
		public PingResult Ping()
		{
			if (!IsActive)
			{
				return new PingResult()
				{
					IsSuccess = false,
					Messages = new[] { Messages.ServiceIsNotActive }
				};
			}
			else
			{
				if (WriteTrace)
				{
					PXTrace.WriteInformation("Ping Request");
				}
				return new PingResult()
				{
					IsSuccess = true,
					Messages = new[] { Messages.ConnectionSuccessfull },

				};
			}
		}

		public GetTaxResult GetTax(GetTaxRequest request)
		{
			if (!IsActive)
			{
				return new GetTaxResult()
				{
					IsSuccess = false,
					Messages = new[] { Messages.ServiceIsNotActive }
				};
			}
			else
			{
				if (WriteTrace)
				{
					PXTrace.WriteInformation("GetTax Request: "+ JsonConvert.SerializeObject(request, Formatting.Indented));
				}
				decimal amount = request.CartItems.Sum(_ => _.Amount);
				decimal taxAmount = amount * TaxPercentage;
				string taxName = request.DestinationAddress.Region + "DummyTax";
				return new GetTaxResult()
				{
					IsSuccess = true,
					Messages = new string[0],
					TaxLines = new TaxLine[] { new TaxLine() { Index = 1, Rate = TaxPercentage, TaxableAmount = amount, TaxAmount = taxAmount } },
					TaxSummary = new TaxDetail[] 
					{ 
						new TaxDetail() 
						{ 
							Rate = TaxPercentage, TaxableAmount = amount, TaxAmount = taxAmount, 
							JurisCode = taxName, JurisName = taxName, TaxName = taxName
						} 
					},
					TotalAmount = amount + taxAmount,
					TotalTaxAmount = taxAmount
				};
			}
		}

		public PostTaxResult PostTax(PostTaxRequest request)
		{
			if (!IsActive)
			{
				return new PostTaxResult()
				{
					IsSuccess = false,
					Messages = new[] { Messages.ServiceIsNotActive }
				};
			}
			else
			{
				if (WriteTrace)
				{
					PXTrace.WriteInformation("PostTax Request: " + JsonConvert.SerializeObject(request, Formatting.Indented));
				}
				return new PostTaxResult()
				{
					IsSuccess = true,
					Messages = new string[0]
				};
			}
		}

		public CommitTaxResult CommitTax(CommitTaxRequest request)
		{
			if (!IsActive)
			{
				return new CommitTaxResult()
				{
					IsSuccess = false,
					Messages = new[] { Messages.ServiceIsNotActive }
				};
			}
			else
			{
				if (WriteTrace)
				{
					PXTrace.WriteInformation("CommitTax Request: " + JsonConvert.SerializeObject(request, Formatting.Indented));
				}
				return new CommitTaxResult()
				{
					IsSuccess = true,
					Messages = new string[0]
				};
			}
		}

		public VoidTaxResult VoidTax(VoidTaxRequest request)
		{
			if (!IsActive)
			{
				return new VoidTaxResult()
				{
					IsSuccess = false,
					Messages = new[] { Messages.ServiceIsNotActive }
				};
			}
			else
			{
				if (WriteTrace)
				{
					PXTrace.WriteInformation("VoidTax Request: " + JsonConvert.SerializeObject(request, Formatting.Indented));
				}
				return new VoidTaxResult()
				{
					IsSuccess = true,
					Messages = new string[0]
				};
			}
		}
		#endregion
	}
}
