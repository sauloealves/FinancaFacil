using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Transactions {
    public class UpdateTransactionUseCase {
        private readonly ITransactionRepository _repository;
        private readonly CreateTransactionUseCase _createUseCase;

        public UpdateTransactionUseCase(
            ITransactionRepository repository,
            CreateTransactionUseCase createUseCase) {
            _repository = repository;
            _createUseCase = createUseCase;
        }

        public async Task ExecuteAsync(Guid transactionId, Guid userId, UpdateTransactionRequest request) {
            var existing = await _repository
                .GetByIdAsync(transactionId, userId);

            if(existing == null)
                throw new InvalidOperationException("Transaction not found.");

            var isStructuralChange = IsStructuralChange(existing, request);

            if(!existing.OccurrenceGroupId.HasValue) {
                await HandleSingleUpdate(userId, existing, request, isStructuralChange);
                return;
            }

            await HandleGroupedUpdate(userId, existing, request, isStructuralChange);
        }

        private static bool IsStructuralChange(
            Domain.Entities.Transaction existing,
            UpdateTransactionRequest request) {
            if(existing.Type.ToString() != request.Type.ToString())
                return true;

            if(existing.OccurrenceType.ToString() != request.OccurrenceType.ToString())
                return true;

            if(request.FromAccountId.HasValue && request.ToAccountId.HasValue)
                return true;

            return false;
        }

        private async Task HandleSingleUpdate(
            Guid userId,
            Transaction existing,
            UpdateTransactionRequest request,
            bool structuralChange) {
            if(structuralChange) {
                existing.Delete();
                await _repository.SaveChangesAsync();

                await _createUseCase.AddAsync(userId, MapToCreate(request));
                return;
            }

            existing.Update(
                request.Description,
                request.Value,
                request.CategoryId,
                request.StartDate
            );

            await _repository.SaveChangesAsync();
        }

        private async Task HandleGroupedUpdate(
            Guid userId,
            Domain.Entities.Transaction existing,
            UpdateTransactionRequest request,
            bool structuralChange) {
            var groupId = existing.OccurrenceGroupId!.Value;

            var group = await _repository
                .GetByGroupAsync(groupId, userId);

            if(request.EditMode == "single") {
                await HandleSingleUpdate(userId, existing, request, structuralChange);
                return;
            }

            if(request.EditMode == "fromBeginning") {
                foreach(var item in group)
                    item.Delete();

                await _repository.SaveChangesAsync();

                await _createUseCase.AddAsync(userId, MapToCreate(request));
                return;
            }

            if(request.EditMode == "fromThis") {
                foreach(var item in group
                    .Where(t => t.Date >= existing.Date))
                    item.Delete();

                await _repository.SaveChangesAsync();

                await _createUseCase.AddAsync(userId, MapToCreate(request));
            }
        }

        private static CreateTransactionRequest MapToCreate(UpdateTransactionRequest request) {
            return new CreateTransactionRequest {
                Description = request.Description,
                Value = request.Value,
                CategoryId = request.CategoryId,
                AccountId = request.AccountId,
                StartDate = request.StartDate,
                OccurrenceType = request.OccurrenceType,
                InstallmentFrom = request.InstallmentFrom,
                InstallmentTo = request.InstallmentTo,
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Recurrence = request.Recurrence,
                EndDate = request.EndDate,
                Type = request.Type
            };
        }

    }
}
