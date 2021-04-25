const handleMoveToClick = async (event) => {
    await handleCopyMove('Move', event);
};

const handleCopyToClick = async (event) => {
    await handleCopyMove('Copy', event);
};

async function handleCopyMove(type, event) {
    if (!state.selectedFolder) {
        alert('Please select a folder');
        return;
    }
    const relativePath = event.target.getAttribute('relativePath');
    const fileName = event.target.getAttribute('resourceName');
    var relativeDestination = document.getElementById('newFolderDestination').value + '\\' + fileName;
    var response = await fetch('/api/File/' + type + '?relativeDestination=' + encodeURIComponent(relativeDestination) + '&relativePath=' + encodeURIComponent(relativePath), {
        method: 'PUT'
    });
    if (response.ok) {
        await refreshFileSystem();
    } else {
        alert('Please ensure destination does not already exist');
    }
}

const handleDownloadClick = async (event) => {
    const relativePath = event.target.getAttribute('relativePath');
    window.location.href = '/api/File/?relativePath=' + encodeURIComponent(relativePath);
};

const handleFileDeleteClick = async (event) => {
    const relativePath = event.target.getAttribute('relativePath');
    if (confirm('Do you wish to delete: ' + relativePath)) {
        var response = await fetch('/api/File/?relativePath=' + encodeURIComponent(relativePath), {
            method: 'DELETE'
        });
        if (response.ok) {
            await refreshFileSystem();
        }
    }
};

const handleFolderDeleteClick = async (event) => {
    const relativePath = event.target.getAttribute('relativePath');
    if (confirm('Do you wish to delete: ' + relativePath)) {
        var response = await fetch('/api/Folder/?relativePath=' + encodeURIComponent(relativePath), {
            method: 'DELETE'
        });
        if (response.ok) {
            await refreshFileSystem();
        }
    }
};