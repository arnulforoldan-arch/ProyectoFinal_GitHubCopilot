(function(){
  window.awExport = {
    downloadCsv: function(filename, csvText){
      try {
        const blob = new Blob([csvText], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        if (navigator.msSaveBlob) { // IE 10+
          navigator.msSaveBlob(blob, filename);
        } else {
          const url = URL.createObjectURL(blob);
          link.href = url;
          link.download = filename;
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
          URL.revokeObjectURL(url);
        }
      } catch (e) { console.error('downloadCsv error', e); }
    },
    downloadHtml: function(filename, html, mime){
      try {
        const blob = new Blob([html], { type: mime || 'application/octet-stream' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
      } catch (e) { console.error('downloadHtml error', e); }
    },
    printHtml: function(html){
      try {
        const w = window.open('', '_blank');
        w.document.open();
        w.document.write(html);
        w.document.close();
        w.focus();
        // small delay ensures styles apply
        setTimeout(function(){ w.print(); /* optionally w.close(); */ }, 250);
      } catch (e) { console.error('printHtml error', e); }
    }
  };
})();
