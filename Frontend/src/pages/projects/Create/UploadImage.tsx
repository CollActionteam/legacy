import React, { useCallback, useState } from 'react';
import { useDropzone } from "react-dropzone";
import { Button } from '../../../components/Button/Button';
import styles from './UploadImage.module.scss';

const UploadImage = ({ name, formik }: any) => {

  const [previewImage, setPreviewImage] = useState<any>(null);
  const [invalid, setInvalid] = useState(false);

  const onDrop = useCallback((acceptedFiles: File[]) => {

    if (acceptedFiles.length !== 1) {
      setInvalid(true);
      return;
    }

    const reader = new FileReader();
    const file = acceptedFiles[0];

    reader.onload = () => {
      setInvalid(false);
      setPreviewImage(reader.result);
      formik.setFieldValue(name, file);
    };

    reader.readAsDataURL(file);    
  }, [name, formik]);

  const {getRootProps, getInputProps, isDragActive} = useDropzone({
    accept: "image/jpeg, image/png, image/gif, image/bmp",
    maxSize: 1024000,
    multiple: false,
    onDrop
  });

  const removeImage = () => {
    setPreviewImage(null);
    formik.setFieldValue(name, null);
  }

  const dropzone = () => (
    <div className={styles.box} { ...getRootProps() }>
      <input { ...getInputProps() }></input>
      {
        <React.Fragment>
          <p className={styles.instruction}>
            { isDragActive 
              ? (<span className={styles.highlight}>Drop it here!</span>) 
              : (<span>Drag and drop anywhere to upload an image, or tap to select a file</span>) 
            }
          </p>
          <small className={`${styles.subInstruction} ${invalid ? styles.invalid : ''}`}>
            It must be a JPG or PNG, no larger than 1 MB.
          </small>
        </React.Fragment>
      }
    </div>
  );

  const preview = () => (
      <React.Fragment>
        <figure className={styles.preview}>
          <img src={previewImage} alt="Preview"></img>
        </figure>
        <div className={styles.previewInstruction}>
          <small className={styles.subInstruction}>
            Try and change your browser size to see if the image is suitable.
          </small>
          <Button variant="ghost" onClick={removeImage}>Clear</Button>      
        </div>
      </React.Fragment>
  );

  return previewImage == null
    ? dropzone()
    : preview();
}

export default UploadImage;