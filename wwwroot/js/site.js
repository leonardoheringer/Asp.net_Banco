// Document ready function
$(document).ready(function() {
    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Format currency inputs
    $('input[type="number"].currency').each(function() {
        $(this).val(parseFloat($(this).val()).toFixed(2);
    });

    // Confirm before executing sensitive actions
    $('.confirm-action').on('click', function(e) {
        if (!confirm($(this).data('confirm-message') || 'Are you sure?')) {
            e.preventDefault();
        }
    });

    // Auto-format account numbers
    $('.account-number').on('input', function() {
        this.value = this.value.replace(/[^0-9]/g, '');
    });
});

// AJAX helper for transaction forms
function submitTransactionForm(form, successCallback) {
    $.ajax({
        type: form.attr('method'),
        url: form.attr('action'),
        data: form.serialize(),
        success: function(response) {
            if (response.success) {
                if (typeof successCallback === 'function') {
                    successCallback(response);
                }
            } else {
                alert(response.message || 'An error occurred');
            }
        },
        error: function() {
            alert('An error occurred while processing your request');
        }
    });
}