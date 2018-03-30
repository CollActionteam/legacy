import * as React from "react";
import * as ReactDOM from "react-dom";
import renderComponentIf from "./renderComponentIf";
import * as Quill from "quill";
//import registerGlobal from "./registerGlobal";
import registerGlobal from "../global/registerGlobal";

function drawRichTextEditor(hiddenInput: HTMLInputElement, quillDiv: string): void {
    var editor = document.getElementById(quillDiv);
   
    var quill = new Quill(editor, {
        modules: {
            toolbar: [
                ['bold', 'italic', 'underline'],
                ['link'],
            ]
        },
        placeholder: 'replace with instructions', //TODO
        theme: 'snow'
    });
    if (hiddenInput.value) {
        quill.setContents(JSON.parse(hiddenInput.value));
    }
    quill.on('selection-change', function (range) {
        if (!range) {
            var delta = quill.getContents();
            hiddenInput.value = JSON.stringify(delta);
        }
    });

}

registerGlobal("drawRichTextEditor", drawRichTextEditor);


