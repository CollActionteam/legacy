import * as React from "react";
import UploadImage from "./UploadImage";
import DropZone from "react-dropzone";
import renderComponentIf from "../global/renderComponentIf";

export default class UploadBanner extends UploadImage {
    constructor(props: {}) {
        super(props);

        this.onDrop = this.onDrop.bind(this);
        this.previewImage = this.previewImage.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.getFileInputElement = this.getFileInputElement.bind(this);
        this.removeBanner = this.removeBanner.bind(this);
    }

    onDrop(accepted: File[], rejected: File[], event: any) {
        super.loadImage(accepted, rejected, event);
    }

    previewImage(type: string, image: any) {
        let background = document.getElementById("project-background");
        background.style.backgroundImage = "url(data:" + type + ";" + image + ")";
    }

    onRejected() {
        super.rejectImage();
    }

    getFileInputElement(): HTMLInputElement {
        return document.getElementsByName("BannerImageUpload")[0] as HTMLInputElement;
    }

    removeBanner() {
        super.resetImage();
        let background = document.getElementById("project-background");
        background.style.backgroundImage = null;
    }

    render() {
        return (
            <div id="banner-image">
                <div style={{display: this.state.preview ? "none" : "block"}}>
                    <DropZone
                        name="BannerImageUpload"
                        className="dropzone"
                        accept="image/jpeg, image/png, image/gif, image/bmp"
                        maxSize={1024000}
                        multiple={false}
                        disablePreview={true}
                        onDrop={this.onDrop}
                        onDropRejected={this.onRejected}
                        rejectClassName="field-validation-error">
                        <h3>Upload banner image</h3>
                        <p className={this.state.invalid ? "field-validation-error" : ""}>Drop or tap. Use jpg, png, gif or bmp. Max. 1 MB</p>
                    </DropZone>
                </div>
                <div style={{display: this.state.preview ? "block" : "none"}}>
                    <p>Try and change your browser size, or rotate your device, to see if the image is suitable.</p>
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
