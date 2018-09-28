import * as React from "react";
import UploadImage, { IUploadImageProps, IUploadImageState } from "./UploadImage";
import DropZone from "react-dropzone";
import renderComponentIf from "../global/renderComponentIf";

export default class UploadBanner extends UploadImage<IUploadImageProps, IUploadImageState> {
    constructor(props: {}) {
        super(props);

        this.state = this.createInitialState();

        this.onDrop = this.onDrop.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.rejectImage = this.rejectImage.bind(this);
        this.createCssImage = this.createCssImage.bind(this);
        this.createSrcImage = this.createSrcImage.bind(this);
        this.getFileInputElement = this.getFileInputElement.bind(this);
    }

    onDrop(accepted: File[], rejected: File[], event: any) {
        this.loadImage(accepted, rejected, event);
    }

    onRejected() {
        this.rejectImage();
    }

    createCssImage(): React.CSSProperties {
        if (this.state.image === null) {
            return {
                backgroundImage: "url(/images/picture.svg)",
                backgroundSize: "120px"
            };
        }
        else {
            return {
                backgroundImage: `url(${this.state.image}`,
                backgroundSize: "cover",
                backgroundPosition: "center"
            };
        }
    }

    createSrcImage(): string {
        return this.state.image as string;
    }

    getFileInputElement(): HTMLInputElement {
        return document.getElementsByName("BannerImageUpload")[0] as HTMLInputElement;
    }

    render() {
        return (
            <div id="project-background" className="col-xs-12 banner" style={this.createCssImage()}>
                <div class="container">
                    <div class="row">
                        <div class="col-md-5 col-xs-12">
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
                                            <h3>Drop your banner image here</h3>
                                            <div className="instructions">
                                                <p className={ this.state.invalid ? "field-validation-error" : "hidden" }>
                                                    This image is not valid. Please edit it or pick another one.
                                                </p>
                                                <p>
                                                    Use jpg, png, gif or bmp. Max. 1 MB.
                                                </p>
                                            </div>
                                        </DropZone>
                                    </div>
                                    <div style={{display: this.state.preview ? "block" : "none"}}>
                                        <div className="image-control">
                                            <div className="text">
                                                <img src="/images/BrowserSize.png"></img>
                                                {this.state.ie11
                                                    ? <p>Image preview works better in newer browsers. <a target="_blank" href="http://outdatedbrowser.com">Check here</a> for upgrades.</p>
                                                    : <p>Try and change your browser size, or rotate your device, to see if the image is suitable.</p>
                                                }
                                            </div>
                                            <div className="buttons">
                                                <input type="button" value="Remove banner" onClick={this.resetImage}></input>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            {this.state.ie11 === true && this.state.preview === true &&
                                <img className="preview" src={this.createSrcImage()}></img>
                            }
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
