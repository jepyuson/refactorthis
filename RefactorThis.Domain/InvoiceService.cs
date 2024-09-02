using System;
using System.Linq;
using RefactorThis.Persistence;
using RefactorThis.Persistence.Entities;

namespace RefactorThis.Domain
{
	public class InvoiceService
	{
		private readonly InvoiceRepository _invoiceRepository;

		public InvoiceService( InvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment( Payment payment )
		{
			var inv = _invoiceRepository.GetInvoice( payment.Reference );

			if ( inv == null )
			{
				throw new InvalidOperationException( "There is no invoice matching this payment" );
			}

			if ( inv.Amount == 0) 
			{
				if(!inv.Payments.Any( )) 
				{
					return "no payment needed";
				}
				throw new InvalidOperationException( "The invoice is in an invalid state, it has an amount of 0 and it has payments." );
			}

			if (inv.Type != InvoiceType.Commercial || inv.Type != InvoiceType.Standard) 
			{
				throw new ArgumentOutOfRangeException( "The invoice type is invalid." );
			}

			if ( inv.Payments != null && inv.Payments.Any( ) )
			{
				if ( inv.Payments.Sum( x => x.Amount ) != 0 && inv.Amount == inv.Payments.Sum( x => x.Amount ) )
				{
					return "invoice was already fully paid";
				}
				
				if ( inv.Payments.Sum( x => x.Amount ) != 0 && payment.Amount > ( inv.Amount - inv.AmountPaid ) )
				{
					return "the payment is greater than the partial amount remaining";
				}

				if (inv.Type == InvoiceType.Commercial) 
				{
					inv.TaxAmount += payment.Amount * 0.14m;
				}

				inv.AmountPaid += payment.Amount;
				inv.Payments.Add( payment );
				_invoiceRepository.SaveInvoice(inv);
				
				if ( ( inv.Amount - inv.AmountPaid ) == payment.Amount )
				{
					return "final partial payment received, invoice is now fully paid";
				}
				else 
				{
					return "another partial payment received, still not fully paid";
				}
			}

			if ( payment.Amount > inv.Amount )
			{
				return "the payment is greater than the invoice amount";
			}

			inv.AmountPaid = payment.Amount;
			inv.TaxAmount = payment.Amount * 0.14m;
			inv.Payments.Add( payment );
            _invoiceRepository.SaveInvoice(inv);

            if ( inv.Amount == payment.Amount ) 
			{
				return "invoice is now fully paid";
			}

			return "invoice is now partially paid";
		}
	}
}