"use strict";

const gulp = require("gulp"),
    rename = require('gulp-rename'),
    fs = require('fs');

        //todo: postinstall

function copyPackageJsToWwwroot(packageName, srcFilename) {
    // Read the package.json file to get the package version
    const packageJson = JSON.parse(fs.readFileSync(`../${packageName}/package.json`));
    const packageVersion = packageJson.version;

    // Set the destination file name
    const destFileName = `${packageName}-${packageVersion}.min.js`;

    // Copy and rename the file
    return gulp.src(`../${packageName}/${srcFileName}`)
        .pipe(rename(destFileName))
        .pipe(gulp.dest('../../wwwroot/js'));
}

gulp.task('copy-packages-js', function() {
    copyPackageJsToWwwroot('govuk-frontend', 'all.js');
    copyPackageJsToWwwroot('familyhubs-frontend', 'all.js');
});

        //todo: delegate from consumer gulp to this gulp?