import React, { useCallback } from 'react';
import { useDropzone } from "react-dropzone";

import styles from './UploadImage.module.scss';

const UploadImage = () => {
  const onDrop = useCallback(acceptedFiles => {
    // Do something with the files
  }, [])
  const {getRootProps, getInputProps, isDragActive} = useDropzone({onDrop})

  return (
    <div className={styles.box} { ...getRootProps() }>
      <input { ...getInputProps() }></input>
      {
        <React.Fragment>
          <p className={styles.instruction}>
            { isDragActive 
              ? (<span className={styles.highlight}>Drop it here!</span>) 
              : (<span>Drag and drop anywhere to upload an image, or click to select a file</span>) 
            }
          </p>
          <small className={styles.subInstruction}>It must be a JPG or PNG, no larger than 1 MB.</small>
        </React.Fragment>
      }
    </div>
  )
}

export default UploadImage;