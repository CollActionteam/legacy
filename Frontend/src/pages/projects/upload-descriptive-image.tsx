import * as React from "react";
import UploadImage, { IUploadImageState } from "../../components/UploadImage";
import { TertiaryButton } from "../../components/Button";
import DropZone from "react-dropzone";

import BrowserSizeImage from "./BrowserSize.png";
import styles from "./create.module.scss";
import { Grid, Container, FormControl, TextField } from "@material-ui/core";
import { Field } from "formik";
import { Section } from "../../components/Section";

export default class UploadDescriptiveImage extends UploadImage<
  {},
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

  uploadCardStyle() {
    if (this.state.image === null) {
      return {};
    }

    return {
      opacity: 0.8,
    };
  }

  createSrcImage(): string {
    return this.state.image as string;
  }

  renderPreviewControl() {
    return (
      <Grid item xs={12} md={5}>
        <Container>
          {this.state.preview && (
            <div className={styles.descriptiveImageDescription}>
              <p>The image description will appear here</p>
              <img src={BrowserSizeImage}></img>
              <p>
                Resize your browser or rotate your device to see if the image
                suitable.
              </p>
              <TertiaryButton onClick={this.resetImage}>
                Remove banner
              </TertiaryButton>
            </div>
          )}
        </Container>
      </Grid>
    );
  }

  render() {
    return (
      <>
        <Grid container>
          {this.renderPreviewControl()}
          <Grid item xs={12} md={7}>
            <Container>
              <div className={styles.uploadDescriptiveImage}>
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
                          <h2>Drop a image / tap to select</h2>
                          {this.state.invalid && (
                            <p className={styles.error}>
                              This image is not valid. Please edit it or pick
                              another one.
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
                  <img
                    className={styles.descriptiveImagePreview}
                    src={this.state.image}
                  ></img>
                )}
              </div>
            </Container>
          </Grid>
        </Grid>
        <Grid container>
          <Grid item xs={12} md={5}></Grid>
          <Grid item xs={12} md={7}>
            <Section className={styles.form}>
              <FormControl>
                <Field
                  name="imageDescription"
                  label="Image description"
                  helperText="A caption for your image"
                  component={TextField}
                ></Field>
              </FormControl>
            </Section>
          </Grid>
        </Grid>
      </>
    );
  }
}

// {this.state.preview && (
//   <Grid item xs={12} md={7}>
//     <img
//       className={styles.descriptiveImagePreview}
//       src={this.state.image}
//     ></img>
//     <Section>
//       <FormControl>
//         <Field
//           name="imageDescription"
//           label="Image description"
//           helperText="Text to accompany your image"
//           component={TextField}
//         ></Field>
//       </FormControl>
//     </Section>
//     )}
//   </Grid>
// )}

