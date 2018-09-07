import * as React from "react";
import UploadImage, { IUploadImageProps, IUploadImageState } from "./UploadImage";
import DropZone from "react-dropzone";
import renderComponentIf from "../global/renderComponentIf";

export default class UploadBanner extends UploadImage<IUploadImageProps, IUploadImageState> {
    constructor(props: {}) {
        super(props);

        this.state = {
            invalid: false,
            preview: false,
            image: null
        };

        this.onDrop = this.onDrop.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.createImage = this.createImage.bind(this);
        this.getFileInputElement = this.getFileInputElement.bind(this);
    }

    onDrop(accepted: File[], rejected: File[], event: any) {
        this.loadImage(accepted, rejected, event);
    }

    onRejected() {
        this.rejectImage();
    }

    createImage(image: any): React.CSSProperties {
        return {
            backgroundImage: `url(${image}`
        };
    }

    getFileInputElement(): HTMLInputElement {
        return document.getElementsByName("BannerImageUpload")[0] as HTMLInputElement;
    }

    render() {
        return (
            <div id="project-background" className="col-xs-12 banner" style={this.state.image}>
                <div id="banner-upload-card">
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
                            <div className="image-control">
                                <div className="text">
                                    <img src="/images/BrowserSize.png"></img>
                                    <p>Try and change your browser size, or rotate your device, to see if the image is suitable.</p>
                                </div>
                                <div className="buttons">
                                    <input type="button" value="Remove banner" onClick={this.resetImage}></input>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <UploadBanner></UploadBanner>,
    document.getElementById("create-project-upload-banner-image")
);
