window.addEventListener('DOMContentLoaded', async (event) => {    
    await getStateFromHash();
    await refreshFileSystem();
    await attachSubmitHandler();    
});

async function attachSubmitHandler() {    
    document.addEventListener('submit', async function (event) {
        event.preventDefault();        
        var response = await fetch('/api/File', {
            method: 'POST',
            body: new FormData(event.target),
        });

        if (response.ok) {
            await refreshFileSystem();
        }
    });
}

async function attachEventListeners() {
    //events to expand and collapse toggles
    const toggles = document.getElementsByClassName('caret');
    Array.prototype.forEach.call(toggles, toggle => {
        if (!toggle.getAttribute('listening')) {
            toggle.addEventListener('click', function () {
                this.parentElement.querySelector('.nested').classList.toggle('active');
                this.classList.toggle('caret-down');
            });
            toggle.setAttribute('listening', 'true');
        }
    });

    //event to populate the folder upload when clicking on folded text
    const folders = document.getElementsByClassName('folder-name');      
    Array.prototype.forEach.call(folders, folder => {
        if (!folder.getAttribute('listening')) {
            folder.addEventListener('click', async function (event) {
                await clickSelectFolder(event.target);
            });
            folder.setAttribute('listening', 'true');
        }
    });      
}

async function clickSelectFolder(folder) {
    const selectedFolders = document.getElementsByClassName('selected-folder');
    Array.prototype.forEach.call(selectedFolders, selectedFolder => {
        selectedFolder.classList.toggle('selected-folder');
    });
    folder.classList.toggle('selected-folder');
    const folderInputs = document.getElementsByClassName('selected-folder-input');
    const fullName = folder.getAttribute('fullName');
    Array.prototype.forEach.call(folderInputs, folderInput => {       
        folderInput.value = fullName;        
    });
    state['selectedFolder'] = fullName;
    await updateUrlHash();    
}

async function performFilter() {
    var filterValue = document.getElementById('filter').value;
    state['fileFilter'] = filterValue;
    await updateUrlHash();
    await refreshFileSystem();
}

async function createNewFolder() {
    var newFolderValue = document.getElementById('newFolder').value;
    var newFolderDestination = document.getElementById('newFolderDestination').value;
    if (!newFolderValue || !newFolderDestination) {
        alert('Please select a desination folder and enter a valid folder name');
        return;
    } else {
        var response = await fetch('/api/Folder?folderName=' + encodeURIComponent(newFolderValue) + '&relativePath=' + encodeURIComponent(newFolderDestination), {
            method: 'POST'
        });
        if (response.ok) {
            await refreshFileSystem();
        }
    }    
}

async function refreshFileSystem() {
    //fetch file system model
    let fileSystemResponse;
    if (state.fileFilter) {       
        document.getElementById('filter').value = state.fileFilter;
        fileSystemResponse = await fetch('/api/FileSystem?filter=' + state.fileFilter);
    } else {
        fileSystemResponse = await fetch('/api/FileSystem');
    }
        
    const fileSystemJson = await fileSystemResponse.json();    
    //add title
    const fileSystemTitle = document.getElementById('fileSystemTitle');
    //clear for when refreshing
    fileSystemTitle.textContent = '';
    const textTitle = document.createTextNode(`Root Path: ${fileSystemJson.FullName}`);
    fileSystemTitle.appendChild(textTitle);

    //populate file system
    const fileSystemTreeview = document.getElementById('fileSystem');
    //clear existing elements and re-populate
    while (fileSystemTreeview.firstChild) {
        fileSystemTreeview.removeChild(fileSystemTreeview.lastChild);
    }
    await renderFileSystemModel(fileSystemTreeview, fileSystemJson);
    await attachEventListeners();
}

async function renderFileSystemModel(rootNode, fileSystemModel) {
    if (fileSystemModel.IsFolder) {
        const listItemNode = document.createElement('LI');
        //create clickable caret
        const folderCaretNode = document.createElement('SPAN');
        folderCaretNode.className = 'caret caret-down';
        listItemNode.appendChild(folderCaretNode);
        //create folder text which can populate upload drop down
        const folderTitleNode = document.createElement('SPAN');
        folderTitleNode.className = 'folder-name';
        folderTitleNode.setAttribute('fullName', fileSystemModel.RelativePath);
        if (fileSystemModel.RelativePath === state.selectedFolder) {            
            await clickSelectFolder(folderTitleNode);
        }
        const folderName = document.createTextNode(`Folder: ${fileSystemModel.Name} - ${fileSystemModel.Bytes} Bytes - ${fileSystemModel.SubDirectoryCount} Sub Directories - ${fileSystemModel.FileCount} Files`);
        folderTitleNode.appendChild(folderName);
        listItemNode.appendChild(folderTitleNode);
        appendActionButton(listItemNode, fileSystemModel, 'Delete Folder', handleFolderDeleteClick);
        //create placeholder for folder contents
        const folderContentNode = document.createElement('UL');
        folderContentNode.className = 'nested active';
        listItemNode.appendChild(folderContentNode);
        
        rootNode.appendChild(listItemNode);
        for (let x = 0; x < fileSystemModel.Children.length; x++) {
            //render children into newly created ul
            await renderFileSystemModel(folderContentNode, fileSystemModel.Children[x]);
        }
    } else {
        const listItemNode = document.createElement('LI');
        const fileName = document.createTextNode(`File: ${fileSystemModel.Name} - ${fileSystemModel.Bytes} Bytes`);
        listItemNode.appendChild(fileName);
        appendActionButton(listItemNode, fileSystemModel, 'Download', handleDownloadClick);
        appendActionButton(listItemNode, fileSystemModel, 'Delete File', handleFileDeleteClick);
        appendActionButton(listItemNode, fileSystemModel, 'Move To Selected Folder', handleMoveToClick);
        appendActionButton(listItemNode, fileSystemModel, 'Copy To Selected Folder', handleCopyToClick);
        rootNode.appendChild(listItemNode);
    }
}

function appendActionButton(listItemNode, fileSystemModel, text, handler) {
    const buttonNode = document.createElement('BUTTON');
    const buttonText = document.createTextNode(text);
    buttonNode.appendChild(buttonText);
    buttonNode.setAttribute('relativePath', fileSystemModel.RelativePath);
    buttonNode.setAttribute('resourceName', fileSystemModel.Name);
    buttonNode.onclick = handler;
    listItemNode.appendChild(buttonNode);
}


