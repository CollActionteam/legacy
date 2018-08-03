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
                        rejectClassName="invalid">
                        <h3>Upload descriptive image</h3>
                        <p className={this.state.invalid ? "invalid" : ""}>Drop or tap. Use jpg, png, gif or bmp. Max. 1 MB</p>
                    </DropZone>
                </div>
                <div style={{display: this.state.preview ? "block" : "none"}}>
                    <div className="col-xs-12 col-md-2">
                        <div className="image-control">
                            <p>The image description will appear here.</p>
                            <p>Try and change your browser size, or rotate your device, to see if the image is suitable.</p>
                            <input type="button" value="Remove" onClick={this.removeBanner}></input>
                        </div>
                    </div>
                    <div className="col-xs-12 col-md-10">
                        <img id="preview" className="pull-right"></img>
                    </div>
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <UploadDescriptiveImage></UploadDescriptiveImage>,
    document.getElementById("create-project-upload-descriptive-image")
);
