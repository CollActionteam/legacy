import * as React from "react";

export interface IUploadImageProps {
}

export interface IUploadImageState {
    invalid: boolean;
    preview: boolean;
    image: any;
    ie11: boolean;
}

export default abstract class UploadImage<P extends IUploadImageProps, S extends IUploadImageState> extends React.Component<P, S> {
    constructor(props: any) {
        super(props);

        this.loadImage = this.loadImage.bind(this);
        this.rejectImage = this.rejectImage.bind(this);
        this.resetImage = this.resetImage.bind(this);
    }

    protected createInitialState(): IUploadImageState {
        return {
            invalid: false,
            preview: false,
            image: null,
            ie11: navigator.userAgent.indexOf("MSIE") !== -1 || navigator.appVersion.indexOf("Trident/") > 0
        };
    }

    protected loadImage(accepted: File[], rejected: File[], event: any) {
        event.persist();
        if (this.state.preview) {
            // On Safari and Chrome, setBannerImageUploadInput triggers this function again.
            // On Firefox it does not... so, if preview is already in progress, we can stop.
            return;
        }

        if (accepted.length !== 1) {
            this.setState({ invalid: true });
            return;
        }

        let that = this;
        this.setState({ invalid: false, preview: true});

        if (event.dataTransfer) {
            // onDrop was triggered from drag and drop event.
            // Add the files from the event to the BannerImageUpload input element.
            let files = event.dataTransfer.files;
            setTimeout(function() {
                // Schedule this for the next event loop.
                // Otherwise, on Chrome and Safari it will trigger onDrop in the same event loop and we won't have the correct state.
                let inputElement = that.getFileInputElement();
                inputElement.files = files;
            });
        }

        // Note: when the user clicked or tapped on the dropzone and selected a file,
        // then the files will already be in the file input element.

        let reader = new FileReader();
        let file = accepted[0];
        reader.onload = function() {
            that.setState({ image: reader.result });
        };
        reader.onabort = this.rejectImage;
        reader.onerror = this.rejectImage;

        reader.readAsDataURL(file);
    }

    abstract getFileInputElement(): HTMLInputElement;

    protected rejectImage() {
        this.setState({ invalid: true, preview: false });
    }
    protected resetImage() {
        let inputElement = this.getFileInputElement();
        inputElement.value = "";

        this.setState({ preview: false, image: null });
    }

    abstract render();
}
