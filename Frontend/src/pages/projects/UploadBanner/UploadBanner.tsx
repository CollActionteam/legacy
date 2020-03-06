import * as React from "react";
import DropZone from "react-dropzone";
import UploadImage, {
  IUploadImageState,
  IUploadImageProps,
} from "../../../components/UploadImage";
import { TertiaryButton } from "../../../components/Button/Button";

import BrowserSizeImage from "../../../assets/BrowserSize.png";
import styles from "../Create/Create.module.scss";

export default class UploadBanner extends UploadImage<
  IUploadImageProps,
  IUploadImageState
> {
  constructor(props: {}) {
    super(props);

    this.state = super.createInitialState();

    this.onDrop = this.onDrop.bind(this);
    this.onRejected = this.onRejected.bind(this);
    this.createCssImage = this.createCssImage.bind(this);
    this.createSrcImage = this.createSrcImage.bind(this);
  }

  onDrop(accepted: File[], _rejected: File[], _event: any) {
    this.loadImage(accepted, (file: File) => {
      this.props.formik.setFieldValue("banner", file);
    });
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
              {({ getRootProps, getInputProps, open }: any) => {
                return (
                  <div
                    {...getRootProps()}
                    className={styles.dropzone}
                    onClick={() => open()}
                  >
                    <h3>Drop banner image / tap to select</h3>
                    <div className={styles.dropzoneInstruction}>
                      {this.state.invalid && (
                        <div className={styles.error}>
                          This image is not valid. Please edit it or pick
                          another one.
                        </div>
                      )}
                      <div className={this.state.invalid ? styles.error : ""}>
                        Use jpg, png, gif or bmp. Max. 1 MB.
                      </div>
                    </div>
                    <input {...getInputProps()}></input>
                  </div>
                );
              }}
            </DropZone>
          )}

          {this.state.preview && (
            <div className={styles.imagePreview}>
              <img src={BrowserSizeImage} alt="Resize your browser"></img>
              <div className={styles.dropzoneInstruction}>
                Resize your browser or rotate your device to see if the image is
                suitable.
              </div>
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
