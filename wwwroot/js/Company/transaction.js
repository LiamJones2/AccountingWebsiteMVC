function openEditModal(id, amount, date, description) {
    document.getElementById("overlay").style.display = "block";
    document.getElementById("transactionModal").style.display = "block";



    document.getElementById('transactionModal').innerHTML = `
                                                    <form asp-action="UpdateTransaction" asp-controller="Company" method="post" id="transactionForm">
                                <label>Amount:</label>
                                                <input type="hidden" name="itemId" value="${id}" />
                                                        <input name="amount" value="${amount}" />
                                                                         <input name="date" value="${date}" />
                                                                        <input name="description" value="${description}" />
                                                <button type="button" onclick="updateTransaction()">Save Changes</button>
                            </form>
                                            <button onclick="closeTransactionModal()">Cancel</button>
                        `;
}

function closeTransactionModal() {
    document.getElementById("overlay").style.display = "none";
    document.getElementById("transactionModal").style.display = "none";
}

function openDeleteModal(id, amount, date, description) {
    document.getElementById("overlay").style.display = "block";
    document.getElementById("transactionModal").style.display = "block";

    document.getElementById('transactionModal').innerHTML = `
                                                    <form>
                                        <label>Amount:</label>
                                                                <input type="hidden" name="itemId" value="${id}" readonly/>
                                                                        <input name="amount" value="${amount}" readonly/>
                                                                                         <input name="date" value="${date}" readonly/>
                                                                                        <input name="description" value="${description}" readonly/>
                                                        <button onclick="deleteTransaction()">Delete Transaction</button>
                                    </form>
                                                    <button onclick="closeTransactionModal()">Cancel</button>
                                `;
}

function openCreateModal(transactionTypeId) {
    document.getElementById("overlay").style.display = "block";
    document.getElementById("transactionModal").style.display = "block";

    document.getElementById('transactionModal').innerHTML = `
                                                            <form>
                                                <label>Amount:</label>
                                                                        <input type="hidden" name="transactionId" value="${transactionTypeId}" readonly/>
                                                                                <input name="amount" />
                                                                                                 <input name="date" />
                                                                                                <input name="description" />
                                                                <button onclick="createTransaction()">Create Transaction</button>
                                            </form>
                                                            <button onclick="closeTransactionModal()">Cancel</button>
                                        `;
}

function updateTransaction() {
    var formData = $('#transactionForm').serialize();

    $.ajax({
        type: 'POST',
        url: '/Company/UpdateTransaction',
        data: formData,
        success: function (response) {
            alert('Transaction updated successfully!');
            console.log(response);
        },
        error: function (error) {
            console.error(error);
        }
    });

    closeTransactionModal();
}

function deleteTransaction() {
    var formData = $('form').serialize();

    $.ajax({
        type: 'POST',
        url: '/Company/DeleteTransaction',
        data: formData,
        success: function (response) {
            alert('Transaction updated successfully!');
            console.log(response);
        },
        error: function (error) {
            console.error(error);
        }
    });

    closeTransactionModal();
}

function createTransaction() {
    var formData = $('form').serialize();

    $.ajax({
        type: 'POST',
        url: '/Company/CreateTransaction',
        data: formData,
        success: function (response) {
            alert('Transaction created successfully!');
            console.log(response);
        },
        error: function (error) {
            console.error(error);
        }
    });

    closeTransactionModal();
}