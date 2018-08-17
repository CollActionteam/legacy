import * as React from "react";
import UploadImage from "./UploadImage";
import DropZone from "react-dropzone";
import renderComponentIf from "../global/renderComponentIf";

export default class UploadDescriptiveImage extends UploadImage {
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
        let background = document.getElementById("preview") as HTMLImageElement;
        background.src = "data:" + type + ";" + image;
    }

    onRejected() {
        super.rejectImage();
    }

    getFileInputElement(): HTMLInputElement {
        return document.getElementsByName("DescriptiveImageUpload")[0] as HTMLInputElement;
    }

    removeBanner() {
        super.resetImage();
        let background = document.getElementById("preview") as HTMLImageElement;
        background.src = null;
    }

    renderImageControl() {
        return (
            <div className="image-control">
                <div className="description">
                    <p>The image description will appear here.</p>
                </div>
                <div className="text">
                    <img src="/images/BrowserSize.png"></img>
                    <p>Try and change your browser size, or rotate your device, to see if the image is suitable.</p>                    
                </div>
                <div className="buttons">
                    <input type="button" value="Remove" onClick={this.removeBanner}></input>
                </div>
            </div>
        );
    }

    render() {
        return (
            <div id="project-image">
                <div className="col-xs-12 col-md-7 col-md-offset-5" style={{display: this.state.preview ? "none" : "block"}}>
                    <DropZone
                        name="DescriptiveImageUpload"
                        className="dropzone"
                        accept="image/jpeg, image/png, image/gif, image/bmp"
                        maxSize={1024000}
                        multiple={false}
                        disablePreview={true}
                        onDrop={this.onDrop}
                        onDropRejected={this.onRejected}
                        rejectClassName="field-validation-error">
                        <h3>Upload descriptive image</h3>
                        <p className={this.state.invalid ? "field-validation-error" : ""}>Drop or tap. Use jpg, png, gif or bmp. Max. 1 MB</p>
                    </DropZone>
                </div>
                <div className="hidden-xs hidden-sm col-md-2" style={{display: this.state.preview ? "block" : "none"}}>
                    { this.renderImageControl() }
                </div>
                <div className="col-xs-12 col-md-10" style={{display: this.state.preview ? "block" : "none"}}>
                    <img id="preview" className="pull-right"></img>
                </div>
                <div className="col-xs-12 hidden-md hidden-lg" style={{display: this.state.preview ? "block" : "none"}}>
                    { this.renderImageControl() }
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <UploadDescriptiveImage></UploadDescriptiveImage>,
    document.getElementById("create-project-upload-descriptive-image")
);
