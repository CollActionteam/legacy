import * as React from "react";
import DropZone from "react-dropzone";
import UploadImage, { IUploadImageState } from "../../components/UploadImage";
import { TertiaryButton } from "../../components/Button";

import BrowserSizeImage from "./BrowserSize.png";
import styles from "./create.module.scss";

export default class UploadBanner extends UploadImage<{}, IUploadImageState> {
  constructor(props: {}) {
    super(props);

    this.state = super.createInitialState();

    this.onDrop = this.onDrop.bind(this);
    this.onRejected = this.onRejected.bind(this);
    this.createCssImage = this.createCssImage.bind(this);
    this.createSrcImage = this.createSrcImage.bind(this);
  }

  onDrop(accepted: File[], rejected: File[], event: any) {
    this.loadImage(accepted, rejected, event);
  }

  onRejected() {
    this.rejectImage();
  }

  createCssImage(): React.CSSProperties {
    if (this.state.image === null) {
      return {};
    }

    return {
      backgroundImage: `url(${this.state.image}`,
      backgroundPosition: "center",
      backgroundRepeat: "no-repeat",
      backgroundSize: "cover",
    };
  }

  createSrcImage(): string {
    return this.state.image as string;
  }

  render() {
    return (
      <div className={styles.projectBanner} style={this.createCssImage()}>
        <div
          className={styles.uploadBanner}
          style={this.state.image ? { opacity: 0.8 } : {}}
        >
          {!this.state.preview && (
            <DropZone
              accept="image/jpeg, image/png, image/gif, image/bmp"
              maxSize={1024000}
              multiple={false}
              onDrop={this.onDrop}
              onDropRejected={this.onRejected}
            >
              {({ getRootProps, getInputProps, open }) => {
                return (
                  <div
                    {...getRootProps()}
                    className={styles.dropzone}
                    onClick={() => open()}
                  >
                    <h2>Drop banner image / tap to select</h2>
                    {this.state.invalid && (
                      <p className={styles.error}>
                        This image is not valid. Please edit it or pick another
                        one.
                      </p>
                    )}
                    <p className={this.state.invalid ? styles.error : ""}>
                      Use jpg, png, gif or bmp. Max. 1 MB.
                    </p>
                    <input {...getInputProps()}></input>
                  </div>
                );
              }}
            </DropZone>
          )}

          {this.state.preview && (
            <div className={styles.bannerPreview}>
              <img src={BrowserSizeImage}></img>
              <p>
                Resize your browser or rotate your device to see if the image is
                suitable.
              </p>
              <TertiaryButton onClick={this.resetImage}>
                Remove banner
              </TertiaryButton>
            </div>
          )}
        </div>
      </div>
    );
  }
}
