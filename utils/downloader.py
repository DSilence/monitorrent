import cgi
import requests

def download(url, **kwargs):
    response = requests.get(url, **kwargs)
    filename = None
    if 'content-disposition' in response.headers:
        content_disposition = response.headers['content-disposition']
        t, params = cgi.parse_header(content_disposition)
        if t == 'attachment' and 'filename' in params:
            filename = params['filename']

    return response.content, filename
