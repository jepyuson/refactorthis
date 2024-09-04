﻿using RefactorThis.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Repositories
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(string reference);

        void SaveInvoice(Invoice invoice);

        void Add(Invoice invoice);
    }
}
