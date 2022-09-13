function downloadFile(token) {
    let url = "/Admin/File/GetDownloadFile?url=" + window.location.href + "&token=" + token;
    window.open(url, '_self');
}
