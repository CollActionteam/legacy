import * as React from "react";
import DropZone from "react-dropzone";
import renderComponentIf from "./renderComponentIf";

interface IUploadBannerProps {
}

interface IUploadBannerState {
    invalid: boolean;
    preview: boolean;
}

export default class UploadBanner extends React.Component<IUploadBannerProps, IUploadBannerState> {
    constructor(props: {}) {
        super(props);

        this.state = {
            invalid: false,
            preview: false
        };

        this.loadBannerImage = this.loadBannerImage.bind(this);
        this.previewBanner = this.previewBanner.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.removeBanner = this.removeBanner.bind(this);
    }

    loadBannerImage(accepted: File[], rejected: File[], event: any) {
        if (this.state.preview) {
            // On Safari and Chrome, setBannerImageUploadInput triggers this this function again.
            // On Firefox it does not... so, if preview is already in progress, we can stop.
            return;
        }

        if (accepted.length !== 1) {
            this.setState({ invalid: true });
            return;
        }

        let that = this;
        this.setState({ preview: true});

        if (event.dataTransfer) {
            // onDrop was triggered from drag and drop event.
            // Add the files from the event to the BannerImageUpload input element.
            let files = event.dataTransfer.files;
            setTimeout(function() {
                // Schedule this for the next event loop.
                // Otherwise, on Chrome and Safari it will trigger onDrop in the same event loop and we won't have the correct state.
                that.setBannerImageUploadInput(files);
            });
        }

        // Note: when the user clicked or tapped on the dropzone and selected a file,
        // then the files will already be in the file input element.

        let reader = new FileReader();
        let file = accepted[0];
        reader.onload = function() {
            that.previewBanner(file.type, reader.result);
        };
        reader.onabort = this.onRejected;
        reader.onerror = this.onRejected;

        reader.readAsDataURL(file);
    }

    setBannerImageUploadInput(files: FileList) {
        let inputElement = document.getElementsByName("BannerImageUpload")[0] as HTMLInputElement;
        inputElement.files = files;
    }

    previewBanner(type: string, image: any) {
        let background = document.getElementById("project-background");
        background.style.backgroundImage = "url(data:" + type + ";" + image + ")";
    }

    onRejected() {
        this.setState({ invalid: true, preview: false });
    }

    removeBanner() {
        let background = document.getElementById("project-background");
        background.style.backgroundImage = null;

        this.resetBannerImageUploadInput();

        this.setState({ preview: false });
    }

    resetBannerImageUploadInput() {
        let inputElement = document.getElementsByName("BannerImageUpload")[0] as HTMLInputElement;
        inputElement.value = "";
    }

    render() {
        return (
            <div>
                <div style={{display: this.state.preview ? "none" : "block"}}>
                    <DropZone
                        name="BannerImageUpload"
                        className="dropzone"
                        accept="image/jpeg, image/png, image/gif, image/bmp"
                        maxSize={1024000}
                        multiple={false}
                        disablePreview={true}
                        onDrop={this.loadBannerImage}
                        onDropRejected={this.onRejected}
                        rejectClassName="invalid">
                        <h3>Upload banner image</h3>
                        <p className={this.state.invalid ? "invalid" : ""}>Drop or tap. Use jpg, png, gif or bmp. Max. 1 MB</p>
                    </DropZone>
                </div>
                <div style={{display: this.state.preview ? "block" : "none"}}>
                    <p>Try and change your browser size, or rotate your device, to see if the image is suitable as banner.</p>
                    <input type="button" value="Remove banner" onClick={this.removeBanner}></input>
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <UploadBanner></UploadBanner>,
    document.getElementById("create-project-upload-banner-image")
);
