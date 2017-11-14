﻿using System;
using System.Collections.Generic;
using System.Linq;
using HospitalDbExtended.Data.Interfaces;
using HospitalDbExtended.Data.Models;
using HospitalDbExtended.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HospitalDbExtended.Data.CommandsModels
{
    public class ListPrescriptionsCommand : Command
    {
        public ListPrescriptionsCommand(HospitalContext context, bool isLogged, int loggedDoctorId, IReader reader, IWriter writer)
            : base(context, isLogged, loggedDoctorId, reader, writer)
        {
        }

        public override void Execute()
        {
            Patient[] patients = this.Context
                .Patients
                .Include(p => p.Visitations)
                .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Medicament)
                .Where(p => p.Visitations.Any(v => v.DoctorId == this.LoggedDoctorId))
                .ToArray();

            if (patients.Length == 0)
            {
                this.Writer.WriteLine(string.Format(InfoMessages.ExtractedEntityCollectionEmpty, "prescriptions"));
            }
            else
            {
                IList<PatientMedicament> prescriptions = new List<PatientMedicament>();

                foreach (Patient patient in patients)
                {
                    foreach (PatientMedicament prescription in patient.Prescriptions)
                    {
                        prescriptions.Add(prescription);
                    }
                }

                if (prescriptions.Count == 0)
                {
                    this.Writer.WriteLine(string.Format(InfoMessages.ExtractedEntityCollectionEmpty, "prescriptions"));
                    this.Writer.Write(Environment.NewLine);
                }
                else
                {
                    this.Writer.WriteLine(string.Format(InfoMessages.ExtractedEntityCollectionIndicator, "prescriptions"));
                    this.Writer.Write(Environment.NewLine);

                    foreach (PatientMedicament prescription in prescriptions)
                    {
                        this.Writer.WriteLine(prescription.ToString());
                    }
                }
            }

            this.Writer.Write(Environment.NewLine);
        }
    }
}