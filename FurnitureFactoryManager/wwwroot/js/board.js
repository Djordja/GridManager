(function () {
    const dragClass = 'dragging';
    const dropClass = 'drag-over';
    let dragContext = { card: null, zone: null };

    function handleDragStart(event) {
        event.dataTransfer.effectAllowed = 'move';
        event.dataTransfer.setData('text/plain', event.target.dataset.taskId);
        event.target.classList.add(dragClass);
        dragContext = {
            card: event.target,
            zone: event.target.closest('[data-dropzone]')
        };
    }

    function handleDragEnd(event) {
        event.target.classList.remove(dragClass);
        document.querySelectorAll('[data-dropzone]').forEach(zone => zone.classList.remove(dropClass));
        dragContext = { card: null, zone: null };
    }

    function handleDragOver(event) {
        event.preventDefault();
        event.dataTransfer.dropEffect = 'move';
        event.currentTarget.classList.add(dropClass);
    }

    function handleDragLeave(event) {
        event.currentTarget.classList.remove(dropClass);
    }

    function handleDrop(event) {
        event.preventDefault();
        const taskId = event.dataTransfer.getData('text/plain');
        const taskCard = document.querySelector(`[data-task-id="${taskId}"]`);
        const newStatus = event.currentTarget.closest('[data-status]').dataset.status;
        const previousZone = dragContext.zone;
        const previousStatus = taskCard ? taskCard.dataset.status : null;

        if (!taskCard || !newStatus) {
            return;
        }

        if (taskCard.dataset.status === newStatus) {
            return;
        }

        event.currentTarget.appendChild(taskCard);
        taskCard.dataset.status = newStatus;
        document.querySelectorAll('[data-dropzone]').forEach(zone => zone.classList.remove(dropClass));
        updateStatus(taskCard, newStatus, previousZone, previousStatus);
    }

    function updateStatus(taskCard, newStatus, previousZone, previousStatus) {
        if (!window.boardSettings || !window.boardSettings.updateUrl) {
            return;
        }

        const taskId = Number(taskCard.dataset.taskId);

        fetch(window.boardSettings.updateUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ taskId, status: newStatus })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Neuspešno čuvanje statusa');
                }
                return response.json();
            })
            .then(() => {
                const toastElement = document.getElementById('statusToast');
                if (!toastElement) {
                    return;
                }

                const toast = bootstrap.Toast.getOrCreateInstance(toastElement);
                toast.show();
            })
            .catch(error => {
                console.error(error);
                if (previousZone && previousStatus) {
                    previousZone.appendChild(taskCard);
                    taskCard.dataset.status = previousStatus;
                }
                document.querySelectorAll('[data-dropzone]').forEach(zone => zone.classList.remove(dropClass));
                alert('Čuvanje statusa nije uspelo. Osvježite stranicu.');
            });
    }

    function setupDragAndDrop() {
        document.querySelectorAll('.task-card').forEach(card => {
            card.addEventListener('dragstart', handleDragStart);
            card.addEventListener('dragend', handleDragEnd);
        });

        document.querySelectorAll('[data-dropzone]').forEach(zone => {
            zone.addEventListener('dragover', handleDragOver);
            zone.addEventListener('dragleave', handleDragLeave);
            zone.addEventListener('drop', handleDrop);
        });
    }

    document.addEventListener('DOMContentLoaded', setupDragAndDrop);
})();
